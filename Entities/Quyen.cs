using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Core.Entities.TaskManagement
{
    public class Quyen : BaseEntity
    {
        [MaxLength(100)]
        public string TenQuyen { get; set; }
        public virtual IEnumerable<QuyenDuAn> QuyenDuAn { get; set; }
    }
}
