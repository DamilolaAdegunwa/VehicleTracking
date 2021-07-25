using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Core.Entities.Auditing
{
    public class FullAuditedEntity
    {
        public virtual DateTimeOffset CreationTime { get; set; }
        public virtual long? CreatorUserId { get; set; }
        public virtual long? DeleterUserId { get; set; }
        public virtual DateTimeOffset? DeletionTime { get; set; }
        public virtual long Id { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTimeOffset? LastModificationTime { get; set; }
        public virtual long? LastModifierUserId { get; set; }
    }
}
