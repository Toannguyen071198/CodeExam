using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Core.Entities.TaskManagement;
using CMS.Core.Enums;
using CMS.Core.Extensions;
using CMS.Core.Interfaces;
using CMS.Core.Interfaces.Services.TaskManagement;
using CMS.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CMS.Core.Services.TaskManagement
{
    public class TrangThaiDuAnService : ITrangThaiDuAnService
    {
        private readonly IRepository<TrangThaiDuAn> _trangThaiDuAnRepository;
        public TrangThaiDuAnService(IRepository<TrangThaiDuAn> trangThaiDuAnRepository)
        {
            _trangThaiDuAnRepository = trangThaiDuAnRepository;
        }

        public async Task<ServiceResult> CreateTrangThaiDuAn(TrangThaiDuAn trangThaiDuAn)
        {
            if (_trangThaiDuAnRepository.TableUntracked.Any(x => x.DuAnId == trangThaiDuAn.DuAnId
                 && x.TrangThaiId == trangThaiDuAn.TrangThaiId))
            {
                return ServiceResult.Failed("Trạng thái này đã có trong dự án");
            }
            await _trangThaiDuAnRepository.AddAsync(trangThaiDuAn);
            return ServiceResult.Success;
        }

        public async Task DeleteTrangThaiDuAn(int id)
        {
            var trangThaiDuAn = await _trangThaiDuAnRepository.GetByIdAsync(id);
            await _trangThaiDuAnRepository.DeleteAsync(trangThaiDuAn);
        }

        public IQueryable<TrangThaiDuAn> GetTrangThaiDuAnByDuAnId(int duAnId, string keywords = null, int? loaiCongViecId = null)
        {
            var query = _trangThaiDuAnRepository.TableUntracked
                                                .Include(x => x.TrangThai)
                                                .Where(x => x.DuAnId == duAnId)
                                                .OrderBy(x => x.TrangThaiId)
                                                .AsQueryable();
            return query;
        }

        public async Task<ServiceResult> UpdateTrangThaiDuAn(TrangThaiDuAn trangThaiDuAn)
        {
            if (_trangThaiDuAnRepository.TableUntracked.Any(x => x.DuAnId == trangThaiDuAn.DuAnId
                 && x.TrangThaiId == trangThaiDuAn.TrangThaiId))
            {
                return ServiceResult.Failed("Trạng thái này đã có trong dự án");
            }
            await _trangThaiDuAnRepository.UpdateAsync(trangThaiDuAn);
            return ServiceResult.Success;
        }
    }
}
