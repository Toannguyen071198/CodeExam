using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Core.Entities.TaskManagement
{
    public class CongViec : BaseEntity
    {
        [MaxLength(500)]
        public string TenCongViec { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayHetHan { get; set; }
        public double TyLeHoanThanh { get; set; }
        public string MoTaCongViec { get; set; }
        public string LienHe { get; set; }
        [MaxLength(50)]
        public string DoUuTien { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public bool IsActive { get; set; }
        public int? DuAnId { get; set; }
        public virtual DuAn DuAn { get; set; }
        public int? TrangThaiId { get; set; }
        public virtual TrangThai TrangThai { get; set; }
        public int? LoaiCongViecId { get; set; }
        public virtual LoaiCongViec LoaiCongViec { get; set; }
        // Id của người tạo ra công việc này
        public int? ThanhVienDuAnId { get; set; }
        public virtual ThanhVienDuAn ThanhVienDuAn { get; set; }
        public virtual IEnumerable<PhanViec> PhanViec { get; set; }
        public virtual IEnumerable<ChiTietCongViec> ChiTietCongViec { get; set; }
        public virtual IEnumerable<TagCongViec> TagCongViec { get; set; }
    }
}
