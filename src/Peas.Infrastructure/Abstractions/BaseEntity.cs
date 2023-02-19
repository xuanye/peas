// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Vulcan.DapperExtensions.ORMapping.MySQL;

namespace Peas.Infrastructure
{
    public class BaseEntity : MySQLEntity
    {
        public virtual void CreateBy(string userId)
        {
            SetValue("CreatedBy", userId);           
            SetValue("CreatedAt", DateTime.Now);

            UpdateBy(userId);

        }
        public virtual void UpdateBy(string userId)
        {
            SetValue("UpdatedBy", userId);           
            SetValue("UpdatedAt", DateTime.Now);
        }      

        private void SetValue(string propertyName, object value)
        {
            var property = GetType().GetProperty(propertyName);
            property?.SetValue(this, value);
        }

    }
}
