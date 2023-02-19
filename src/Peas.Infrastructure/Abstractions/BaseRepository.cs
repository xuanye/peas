// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license


using DotBPE.Baseline.Extensions;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Vulcan.DapperExtensions;
using Vulcan.DapperExtensions.Contract;
using Vulcan.DapperExtensions.ORMapping;

namespace Peas.Infrastructure
{
    public class BaseRepository : Vulcan.DapperExtensions.ORMapping.MySQL.MySQLRepository, IRepository
    {
        private static ILogger<SQLMetrics> _metricLogger;
        private static readonly ConcurrentDictionary<string, string> _allColumnsSqlCache = new();

        private static readonly ConcurrentDictionary<Type, EntityColumnMeta> _tableKeyCache = new();
        private static readonly ConcurrentDictionary<Type, string> _tableNameCache = new();
        public BaseRepository(IConnectionManagerFactory factory,string connectionString, ILoggerFactory loggerFactory) :
            base(factory, connectionString, null)
        {
            _metricLogger ??= loggerFactory.CreateLogger<SQLMetrics>();
        }

        protected override ISQLMetrics CreateSQLMetrics()
        {
            return new SQLMetrics(_metricLogger!);
        }




        public IUnitOfWork CreateUnitOfWork(bool isTrans = false)
        {
            IScope innerScope = isTrans ? base.BeginTransScope() : base.BeginConnectionScope();

            return new DefaultUnitOfWork(innerScope);

        }

        public Task<List<TEntity>> FindAllAsync<TEntity>() where TEntity : AbstractBaseEntity
        {
            var sql = $"select {GetAllColumns<TEntity>()} from {GetTableName<TEntity>()}";

            return base.QueryAsync<TEntity>(sql, null);
        }

        public Task<TEntity> FindAsync<TEntity>(int id) where TEntity : AbstractBaseEntity
        {
            var meta = GetTablePrimaryKeyColumn<TEntity>();



            var sql = $"SELECT {GetAllColumns<TEntity>()} FROM {GetTableName<TEntity>()} WHERE `{meta.ColumnName}`={id}";

        

            return base.GetAsync<TEntity>(sql,null);
        }

        public Task<List<TEntity>> FindByConditionAsync<TEntity>(IDictionary<string, string> conditions) where TEntity : AbstractBaseEntity
        {
            var where = BuildFilters<TEntity>(conditions);

            var sql = $"SELECT {GetAllColumns<TEntity>()} FROM {GetTableName<TEntity>()}";

            if (string.IsNullOrEmpty(where))
            {
                sql += " " + where;
            }

            return base.QueryAsync<TEntity>(sql, null);
        }

        public async Task<int> InsertAsync<TEntity>(TEntity entity) where TEntity : AbstractBaseEntity
        {
            var newId = await base.InsertAsync(entity);

            return (int)newId;
        }

        public Task RemoveByConditionAsync<TEntity>(Dictionary<string, string> conditions)
        {
            var where = BuildFilters<TEntity>(conditions);

            var sql = $"DELETE FROM {GetTableName<TEntity>()}";

            if (string.IsNullOrEmpty(where))
            {
                sql += " " + where;
            }
            return base.ExecuteAsync(sql, null);
        }

        public Task RemoveByIdAsync<TEntity>(int id)
        {
            var meta = GetTablePrimaryKeyColumn<TEntity>();

            var sql = $"DELETE FROM {GetTableName<TEntity>()} WHERE `{meta.ColumnName}`={id}";

            return base.ExecuteAsync(sql, null);
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : AbstractBaseEntity
        {
            return base.UpdateAsync(entity);
        }




        #region Static


        /// <summary>
        /// Query all columns of the entity's corresponding table, using comma separation
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected static string GetAllColumns<T>(string prefix = "")
        {
            var entityType = typeof(T);
            string t = entityType.FullName + prefix;
            if (!_allColumnsSqlCache.ContainsKey(t))
            {
                var columns = GetColumns(entityType);
                string sql = GetSelectColumnsSql(columns, prefix);
                _allColumnsSqlCache.TryAdd(t, sql);
            }
            return _allColumnsSqlCache[t];
        }

        static protected string GetSelectColumnsSql(List<string> columns, string prefix)
        {
            prefix ??= "";
            if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith('.'))
            {
                prefix += ".";
            }
            List<string> allColumns = columns.Select(x => $"{prefix}`{x}`").ToList();
            return " " + string.Join(",", allColumns) + " ";
        }

        static protected List<string> GetColumns(Type entityType)
        {
            var meta = EntityReflect.GetDefineInfoFromType(entityType);
            var columns = new List<string>();
            foreach (var colMeta in meta.Columns)
            {
                columns.Add(colMeta.ColumnName);
            }

            return columns;
        }

        static protected EntityColumnMeta GetTablePrimaryKeyColumn<T>()
        {
            var entityType = typeof(T);
            var meta = EntityReflect.GetDefineInfoFromType(entityType);
            if (_tableKeyCache.TryGetValue(entityType, out var metaCol))
            {
                return metaCol;
            }
            metaCol = meta.Columns.Find(x => x.PrimaryKey);

            if (metaCol == null)
            {
                throw new InvalidOperationException($"{entityType.FullName} is not a POEntity");
            }
            _tableKeyCache.TryAdd(entityType, metaCol);

            return metaCol;
        }

        static protected string GetTableName<T>()
        {
            var entityType = typeof(T);
            if (_tableNameCache.TryGetValue(entityType, out var tableName))
            {
                return tableName;
            }
            var tableNameAttribute = entityType.GetCustomAttribute<TableNameAttribute>();

            if (tableNameAttribute == null)
            {
                throw new InvalidOperationException($"{entityType.FullName} is not a POEntity");
            }

            _tableNameCache.TryAdd(entityType, tableNameAttribute.TableName);
            return tableNameAttribute.TableName;
        }

        static protected string BuildFilters<T>(IDictionary<string, string> filters)
        {
            var sb = new StringBuilder();
            var entityType = typeof(T);
            foreach (var kv in filters)
            {
                BuildFilter(sb, kv, entityType);
            }

            return sb.ToString();
        }

        private static void BuildFilter(StringBuilder sb, KeyValuePair<string, string> kv, Type entityType)
        {
            var prefix = "";
            var jsonName = kv.Key;
            var fieldValue = kv.Value;
            sb.Append(" AND ");


            var forceEqual = false;
            var notEqual = false;
            if (jsonName.StartsWith("!"))
            {
                forceEqual = true;
                jsonName = jsonName[1..]; //Substring(1)
            }
            else if (jsonName.StartsWith("~"))
            {
                notEqual = true;
                jsonName = jsonName[1..];
            }

            var propName = jsonName.ToPascalCase();

            var fieldProp = entityType.GetProperty(propName);
            if (fieldProp == null)
            {
                throw new InvalidOperationException($"Field{jsonName} is not existed");
            }

            var mapAttr = fieldProp.GetCustomAttribute<MapFieldAttribute>();
            if (mapAttr == null)
            {
                throw new InvalidOperationException($"Field{jsonName} is not existed");
            }
            var fieldName = mapAttr.MapFieldName;

            if (fieldValue == "<null>")
            {
                sb.AppendFormat("{1}`{0}` is null ", fieldName, prefix);
            }
            else if (fieldValue == "<default>")
            {
                sb.AppendFormat(
                    fieldProp.PropertyType.IsValueType
                        ? "( {1}`{0}`=0 OR {1}`{0}` is null)"
                        : "( {1}`{0}`='' OR {1}`{0}` is null)", fieldName, prefix);
            }
            else if (fieldProp.PropertyType == typeof(string))
            {
                if (!forceEqual)
                {
                    sb.AppendFormat("{2}`{0}` LIKE '%{1}%'", fieldName, ClearSafeStringParams(fieldValue), prefix);
                }
                else
                {
                    sb.AppendFormat("{2}`{0}`='{1}'", fieldName, ClearSafeStringParams(fieldValue), prefix);
                }
            }
            else if (fieldProp.PropertyType == typeof(DateTime))
            {
                var valueParts = fieldValue.Split(',');
                if (valueParts.Length > 1)
                {
                    sb.AppendFormat("({3}`{0}`>='{1:yyyy-MM-dd HH:mm:ss}' AND {3}`{0}`<'{2:yyyy-MM-dd HH:mm:ss}')",
                        fieldName, Convert.ToDateTime(valueParts[0]), Convert.ToDateTime(valueParts[1]), prefix);
                }
                else
                {
                    sb.AppendFormat("{2}`{0}`>='{1}'", fieldName, ClearSafeStringParams(fieldValue), prefix);
                }
            }
            else if (notEqual)
            {
                sb.AppendFormat("{2}`{0}`<>'{1}'", fieldName, ClearSafeStringParams(fieldValue), prefix);
            }
            else
            {
                sb.AppendFormat("{2}`{0}`={1}", fieldName, ClearSafeStringParams(fieldValue), prefix);
            }
        }

        private static string ClearSafeStringParams(string input)
        {
            return !string.IsNullOrEmpty(input) ? input.Replace("--", "").Replace("'", "").Replace(";", "") : "";
        }

        #endregion
    }
}
