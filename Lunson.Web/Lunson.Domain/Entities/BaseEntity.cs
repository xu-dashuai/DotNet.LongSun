using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class BaseEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        public virtual string ID { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string CreatedBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public virtual DateTime? ModifiedOn { get; set; }
        /// <summary>
        /// 是否已删除
        /// 0 未删除
        /// 1 已删除
        /// </summary>
        [DataMember]
        public virtual YesOrNo IsDeleted { get; set; }
        /// <summary>
        /// 复制可修改属性
        /// </summary>
        /// <param name="entity"></param>
        public void CopyProperty(BaseEntity entity)
        {
            var curType=this.GetType();
            var tarType = entity.GetType();
            var baseType=typeof(BaseEntity);

            var propertyList = (from x in curType.GetProperties()
                                join y in tarType.GetProperties() on x.Name equals y.Name
                                where baseType.GetProperties().Select(a => a.Name).Contains(x.Name) == false
                                select new
                                {
                                    cur=x,
                                    tar=y
                                });

            foreach (var p in propertyList)
            {
                try
                {
                    p.cur.SetValue(this, p.tar.GetValue(entity, null), null);
                }
                catch (Exception ex)
                { }
            }
        }
    }
}
