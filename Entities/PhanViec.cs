using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Core.Entities.TaskManagement
{
    public class PhanViec : BaseEntity
    {
        public int? CongViecId { get; set; }
        public virtual CongViec CongViec { get; set; }
        public int? ThanhVienDuAnId { get; set; }
        public virtual ThanhVienDuAn ThanhVienDuAn { get; set; }
        public DateTime NgayPhanViec { get; set; }
    }
}
