using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Core.Entities.TaskManagement
{
    public class ChiTietCongViec : BaseEntity
    {
        public int? CongViecId { get; set; }
        public virtual CongViec CongViec { get; set; }
        public int? TrangThaiId { get; set; }
        public virtual TrangThai TrangThai { get; set; }
        [MaxLength(500)]
        public string TenChiTietCongViec { get; set; }
        public DateTime NgayHetHan { get; set; }
        public virtual IEnumerable<LichSuChiTietCongViec> LichSuChiTietCongViec { get; set; }
        public virtual IEnumerable<BaoCao> BaoCao { get; set; }
    }
}
