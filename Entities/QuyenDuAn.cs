using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Core.Entities.TaskManagement
{
    public class QuyenDuAn : BaseEntity
    {
        public int DuAnId { get; set; }
        public virtual DuAn DuAn { get; set; }
        public int? QuyenId { get; set; }
        public virtual Quyen Quyen { get; set; }
        public virtual IEnumerable<ThanhVienDuAn> ThanhVienDuAn { get; set; }
    }
}
