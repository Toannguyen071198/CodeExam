using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Core.Entities;
using CMS.Core.Entities.TaskManagement;
using CMS.Core.Enums;
using CMS.Core.Extensions;
using CMS.Core.Interfaces;
using CMS.Core.Interfaces.Services.TaskManagement;
using CMS.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CMS.Core.Services.TaskManagement
{
    public class CongViecService : ICongViecService
    {
        private readonly IRepository<CongViec> _congViecRepository;
        private readonly IRepository<PhanViec> _phanVieccRepository;
        private readonly IRepository<ChiTietCongViec> _chiTietCongViecRepository;
        private readonly IRepository<BaoCao> _baoCaoRepository;
        private readonly IRepository<LichSuChiTietCongViec> _lichSuChiTietCongViecRepository;
        private readonly IRepository<ThanhVienDuAn> _thanhVienDuAnRepository;
        private readonly IRepository<TrangThaiDuAn> _trangThaiDuAnRepository;
        private readonly IRepository<TagCongViec> _tagCongViecRepository;
        private readonly IRepository<HocVien> _hocVienRepository;
        public CongViecService(IRepository<CongViec> congViecRepository, IRepository<PhanViec> phanViecRepository,
            IRepository<ChiTietCongViec> chiTietCongViecRepository, IRepository<BaoCao> baoCaoRepository,
            IRepository<LichSuChiTietCongViec> lichSuChiTietCongViecRepository,
            IRepository<ThanhVienDuAn> thanhVienDuAnRepository,
            IRepository<TrangThaiDuAn> trangThaiDuAnRepository,
            IRepository<TagCongViec> tagCongViecRepository,
            IRepository<HocVien> hocVienRepository)
        {
            _congViecRepository = congViecRepository;
            _phanVieccRepository = phanViecRepository;
            _chiTietCongViecRepository = chiTietCongViecRepository;
            _baoCaoRepository = baoCaoRepository;
            _lichSuChiTietCongViecRepository = lichSuChiTietCongViecRepository;
            _thanhVienDuAnRepository = thanhVienDuAnRepository;
            _trangThaiDuAnRepository = trangThaiDuAnRepository;
            _tagCongViecRepository = tagCongViecRepository;
            _hocVienRepository = hocVienRepository;
        }

        public async Task CreateCongViec(CongViec congViec, int hocVienId)
        {
            if (_thanhVienDuAnRepository.TableUntracked
                                        .Include(x => x.QuyenDuAn)
                                        .Any(x => x.HocVienId == hocVienId
                                                  && x.QuyenDuAn.DuAnId == congViec.DuAnId))
            {
                var thanhVienTao = _thanhVienDuAnRepository.TableUntracked
                                        .Include(x => x.QuyenDuAn)
                                        .FirstOrDefault(x =>
                                                  x.HocVienId == hocVienId
                                               && x.QuyenDuAn.DuAnId == congViec.DuAnId);
                congViec.ThanhVienDuAnId = thanhVienTao != null ? (int?)thanhVienTao.Id : null;
            }
            var dSTrangThaiDuAn = _trangThaiDuAnRepository
                .TableUntracked
                .Where(x => x.DuAnId == congViec.DuAnId);
            if (dSTrangThaiDuAn != null)
            {
                congViec.TrangThaiId = dSTrangThaiDuAn.Select(x => x.TrangThaiId).Min();
            }
            congViec.NgayTao = DateTime.Now;
            await _congViecRepository.AddAsync(congViec);

            PhanViec phanViec = new PhanViec()
            {
                CongViecId = congViec.Id,
                ThanhVienDuAnId = congViec.ThanhVienDuAnId,
                NgayPhanViec = DateTime.Now
            };
            await _phanVieccRepository.AddAsync(phanViec);
        }
        public async Task UpdateCongViec(CongViec congViec)
        {
            await _congViecRepository.UpdateAsync(congViec);
        }

        public async Task DeleteCongViec(int congViecId)
        {
            var congViec = await _congViecRepository.GetByIdAsync(congViecId);
            var phanViec = _phanVieccRepository.TableUntracked.Where(x => x.CongViecId == congViecId);
            await _phanVieccRepository.DeleteRangeAsync(phanViec);
            var chiTietCongViec = _chiTietCongViecRepository.TableUntracked.Where(x => x.CongViecId == congViecId).ToList();
            for (int i = 0; i < chiTietCongViec.Count; i++)
            {
                var lichSuChiTietCongViec = _lichSuChiTietCongViecRepository.TableUntracked
                    .Where(x => x.ChiTietCongViecId == chiTietCongViec[i].Id);
                await _lichSuChiTietCongViecRepository.DeleteRangeAsync(lichSuChiTietCongViec);
                var baoCao = _baoCaoRepository.TableUntracked.Where(x => x.ChiTietCongViecId == chiTietCongViec[i].Id);
                await _baoCaoRepository.DeleteRangeAsync(baoCao);

            }
            await _chiTietCongViecRepository.DeleteRangeAsync(chiTietCongViec);
            var tagCongViec = _tagCongViecRepository.TableUntracked.Where(x => x.CongViecId == congViecId);
            await _tagCongViecRepository.DeleteRangeAsync(tagCongViec);
            await _congViecRepository.DeleteAsync(congViec);
        }

        public IQueryable<CongViec> GetCongViec(string keywords = null,
                                                int? duAnId = null,
                                                int? trangThaiId = null,
                                                int? loaiCongViecId = null,
                                                DateTime? TuNgay = null,
                                                DateTime? DenNgay = null,
                                                int? tagId = null,
                                                int? nguoiLamId = null)
        {
            var query = _congViecRepository.TableUntracked
                                           .Include(x => x.LoaiCongViec)
                                           .Include(x => x.TagCongViec)
                                           .ThenInclude(x => x.Tag)
                                           .Include(x => x.PhanViec)
                                           .ThenInclude(x => x.ThanhVienDuAn)
                                           .ThenInclude(x => x.HocVien)
                                           .AsNoTracking();
            if (duAnId.HasValue)
            {
                query = query.Where(x => x.DuAnId == duAnId);
            }
            if (keywords.HasValue())
            {
                query = query.Where(congViec => congViec.TenCongViec.ToLower().Contains(keywords.ToLower()));
            }
            if (trangThaiId.HasValue)
            {
                query = query.Where(x => x.TrangThaiId == trangThaiId);
            }
            if (loaiCongViecId.HasValue)
            {
                query = query.Where(x => x.LoaiCongViecId == loaiCongViecId);
            }
            if (TuNgay.HasValue)
            {
                query = query.Where(x => x.NgayHetHan.Date >= TuNgay.Value.Date);
            }
            if (DenNgay.HasValue)
            {
                query = query.Where(x => x.NgayHetHan.Date <= DenNgay.Value.Date);
            }
            if (tagId.HasValue)
            {
                query = query.Where(x => x.TagCongViec.Any(y => y.TagId == tagId));
            }
            if (nguoiLamId.HasValue)
            {
                query = query.Where(x => x.PhanViec.Any(pv => pv.ThanhVienDuAn.HocVienId == nguoiLamId));
            }
            return query.OrderByDescending(x => x.ThanhVienDuAnId.HasValue)
                        .ThenBy(x => x.NgayHetHan)
                        .AsNoTracking();
        }
        public async Task<CongViec> GetCongViecById(int id)
        {
            return await _congViecRepository.TableUntracked.Include(x => x.TagCongViec).ThenInclude(x => x.Tag)
                                           .FirstOrDefaultAsync(x => x.Id == id);
        }
        public IEnumerable<CongViec> GetCongViecByTrangThaiIdAndDuAnId(int? trangThaiId, int? duAnId)
        {
            return _congViecRepository.TableUntracked.Where(x => x.TrangThaiId == trangThaiId && x.DuAnId == duAnId).AsEnumerable();
        }

        public async Task<int> GetSoLuongCongViecByTrangThaiIdAndDuAnId(int trangThaiId, int duAnId)
        {
            return await _congViecRepository.TableUntracked.CountAsync(x => x.TrangThaiId == trangThaiId && x.DuAnId == duAnId);

        }

        public async Task CreateMarketingHocVien(CongViec congViec, int hocVienId, int creatorId)
        {
            if (_thanhVienDuAnRepository.TableUntracked
                                        .Include(x => x.QuyenDuAn)
                                        .Any(x => x.HocVienId == creatorId
                                                  && x.QuyenDuAn.DuAnId == congViec.DuAnId))
            {
                var thanhVienTao = _thanhVienDuAnRepository.TableUntracked
                                        .Include(x => x.QuyenDuAn)
                                        .FirstOrDefault(x =>
                                                  x.HocVienId == creatorId
                                               && x.QuyenDuAn.DuAnId == congViec.DuAnId);
                if (thanhVienTao != null)
                    congViec.ThanhVienDuAnId = thanhVienTao.Id;
            }
            var dSTrangThaiDuAn = _trangThaiDuAnRepository
                                    .TableUntracked
                                    .Where(x => x.DuAnId == congViec.DuAnId);
            if (!dSTrangThaiDuAn.IsNullOrEmpty())
            {
                congViec.TrangThaiId = dSTrangThaiDuAn
                                    .OrderBy(x => x.ThoiGianTao)
                                    .Select(x => x.TrangThaiId)
                                    .Min();
            }
            congViec.NgayTao = DateTime.Now;
            congViec.NgayHetHan = DateTime.Now.AddDays(5);
            congViec.LoaiCongViecId = 5;
            congViec.TyLeHoanThanh = 0;
            congViec.DoUuTien = "cao";
            congViec.IsActive = true;
            await _congViecRepository.AddAsync(congViec);

            PhanViec phanViec = new PhanViec()
            {
                CongViecId = congViec.Id,
                ThanhVienDuAnId = congViec.ThanhVienDuAnId,
                NgayPhanViec = DateTime.Now
            };
            await _phanVieccRepository.AddAsync(phanViec);

            var hocVien = _hocVienRepository.TableUntracked.FirstOrDefault(x => x.Id == hocVienId);
            if (hocVien != null)
            {
                hocVien.DaMarketing = true;
                await _hocVienRepository.UpdateAsync(hocVien);
            }
        }
    }
}
