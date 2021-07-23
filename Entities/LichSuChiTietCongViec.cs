using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Core.Entities.TaskManagement
{
    public class LichSuChiTietCongViec : BaseEntity
    {
        public int? TrangThaiId { get; set; }
        public virtual TrangThai TrangThai { get; set; }
        public int? ChiTietCongViecId { get; set; }
        public virtual ChiTietCongViec ChiTietCongViec { get; set; }
        public int? ThanhVienDuAnId { get; set; }
        public virtual ThanhVienDuAn ThanhVienDuAn { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int? ThuTu { get; set; }
        public string? GhiChu { get; set; }
    }
}
