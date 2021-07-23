using CMS.Core.Entities;
using CMS.Core.Entities.TaskManagement;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Interfaces.Services.TaskManagement
{
    public interface ICongViecService
    {
        public IQueryable<CongViec> GetCongViec(string keywords = null,
                                                int? duAnId = null,
                                                int? trangThaiId = null,
                                                int? loaiCongViecId = null,
                                                DateTime? TuNgay = null,
                                                DateTime? DenNgay = null,
                                                int? tagId = null,
                                                int? nguoiLamId = null);
        public Task<int> GetSoLuongCongViecByTrangThaiIdAndDuAnId(int trangThaiId, int duAnId);
        public IEnumerable<CongViec> GetCongViecByTrangThaiIdAndDuAnId(int? trangThaiId, int? duAnId);
        public Task CreateCongViec(CongViec congViec, int hocVienId);
        public Task UpdateCongViec(CongViec congViec);
        public Task DeleteCongViec(int congViecId);
        public Task<CongViec> GetCongViecById(int id);
        public Task CreateMarketingHocVien(CongViec congViec, int hocVienId, int creatorId);
    }
}
