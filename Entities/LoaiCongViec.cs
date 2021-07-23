using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Core.Entities.TaskManagement
{
    public class LoaiCongViec : BaseEntity
    {
        [MaxLength(100)]
        public string TenLoaiCongViec { get; set; }
        public virtual IEnumerable<CongViec> CongViec { get; set; }
    }
}
