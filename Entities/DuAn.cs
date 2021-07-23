using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Core.Entities.TaskManagement
{
    public class DuAn : BaseEntity
    {
        [MaxLength(500)]
        public string TenDuAn { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public DateTime? ThoiGianCapNhat { get; set; }
        public DateTime? ThoiGianHetHan { get; set; }
        public string MoTa { get; set; }
        public virtual IEnumerable<TrangThaiDuAn> TrangThaiDuAn { get; set; }
        public virtual IEnumerable<QuyenDuAn> QuyenDuAn { get; set; }
        public virtual IEnumerable<CongViec> CongViec { get; set; }
    }
}
