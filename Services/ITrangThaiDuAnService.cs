using CMS.Core.Entities.TaskManagement;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Interfaces.Services.TaskManagement
{
    public interface ITrangThaiDuAnService
    {
        IQueryable<TrangThaiDuAn> GetTrangThaiDuAnByDuAnId(int duAnId, string tenCongViec = null, int? loaiCongViecId = null);
        Task<ServiceResult> CreateTrangThaiDuAn(TrangThaiDuAn trangThaiDuAn);
        Task<ServiceResult> UpdateTrangThaiDuAn(TrangThaiDuAn trangThaiDuAn);
        Task DeleteTrangThaiDuAn(int id);
    }
}
