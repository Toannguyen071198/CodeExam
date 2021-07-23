using CMS.Core.Entities.TaskManagement;
using CMS.Core.Enums;
using CMS.Core.Interfaces.Services.TaskManagement;
using CMS.Infrastructure;
using CMS.Infrastructure.Data;
using CMS.Web.ApiModels;
using CMS.Web.ApiModels.TaskManagement;
using CMS.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.Apis.TaskManagement
{
    public class CongViecController : BaseApiController
    {
        private readonly ICongViecService _congViecService;
        private readonly IChiTietCongViecService _chiTietCongViecService;
        private readonly IUserService _userService;
        public CongViecController(ICongViecService congViecService, IChiTietCongViecService chiTietCongViecService
            , IPhanViecService phanViecService, IUserService userService)
        {
            _congViecService = congViecService;
            _chiTietCongViecService = chiTietCongViecService;
            _userService = userService;
        }

        [ProducesResponseType(typeof(PagedResult<CongViecDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, Authorize]
        public async Task<IActionResult> GetCongViec(
            [FromQuery] string keywords = null,
            [FromQuery] int? duAnId = null,
            [FromQuery] int? trangThaiId = null,
            [FromQuery] int? loaiCongViecId = null,
            [FromQuery] DateTime? tuNgay = null,
            [FromQuery] DateTime? denNgay = null,
            [FromQuery] Pagination pagination = null)
        {
            var query = _congViecService.GetCongViec(keywords, duAnId, trangThaiId, loaiCongViecId, tuNgay, denNgay);
            var congViec = PagedList.Create(query, pagination.Page - 1, pagination.ItemsPerPage);
            pagination.TotalItems = congViec.TotalCount;
            var congViecDTO = congViec.Select(CongViecDTO.FromEntity);
            var listCongViec = congViecDTO.ToList();
            for (int i = 0; i < listCongViec.Count; i++)
            {
                listCongViec[i].SoLuongChiTietCongViec = await _chiTietCongViecService.GetSoLuongChiTietCongViecByCongViecId(listCongViec[i].Id);
                listCongViec[i].SoLuongChiTietCongViecHoanThanh = await _chiTietCongViecService
                    .GetSoLuongChiTietCongViecByCongViecIdAndTrangThaiId(listCongViec[i].Id, 4);
            }
            congViecDTO = listCongViec;
            var result = new PagedResult<CongViecDTO>(pagination, congViecDTO);
            return Ok(result);
        }

        [ProducesResponseType(typeof(CongViecDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCongViecById(int id)
        {
            var congViec = await _congViecService.GetCongViecById(id);
            var result = CongViecDTO.FromEntity(congViec);
            return Ok(result);
        }

        [ProducesResponseType(typeof(CongViecDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, Authorize]
        public async Task<IActionResult> CreateCongViec(CongViecDTO congViecDTO)
        {
            var hocVien = await _userService.GetHocVienByUsername(User.Identity.Name);
            if (hocVien == null) return Unauthorized();
            var congViec = congViecDTO.ToEntity();
            await _congViecService.CreateCongViec(congViec, hocVien.Id);
            return Ok(congViec);
        }

        [ProducesResponseType(typeof(CongViecDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("marketing/{hocVienId}"), Authorize(Roles = Roles.ADMIN_TRO_GIANG)]
        public async Task<IActionResult> CreateMarketingHocVien(int hocVienId, CongViecDTO congViecDTO)
        {
            var hocVien = await _userService.GetHocVienByUsername(User.Identity.Name);
            if (hocVien == null) return Unauthorized();
            var congViec = congViecDTO.ToEntity();
            await _congViecService.CreateMarketingHocVien(congViec, hocVienId, hocVien.Id);
            return Ok(congViec);
        }
        [ProducesResponseType(typeof(CongViecDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateCongViec(int id, [FromBody] CongViecDTO congViecDTO)
        {
            congViecDTO.NgayCapNhat = DateTime.Now;
            if (congViecDTO.TrangThaiId == 4)
            {
                congViecDTO.NgayHoanThanh = DateTime.Now;
            }
            congViecDTO.SoLuongChiTietCongViec = await _chiTietCongViecService.GetSoLuongChiTietCongViecByCongViecId(congViecDTO.Id);
            congViecDTO.SoLuongChiTietCongViecHoanThanh = await _chiTietCongViecService
                .GetSoLuongChiTietCongViecByCongViecIdAndTrangThaiId(congViecDTO.Id, 4);
            if (congViecDTO.SoLuongChiTietCongViec == 0)
            {
                congViecDTO.TyLeHoanThanh = 0;
            }
            else
            {
                congViecDTO.TyLeHoanThanh = ((100 / (double)(congViecDTO.SoLuongChiTietCongViec)) * (double)congViecDTO.SoLuongChiTietCongViecHoanThanh);
                congViecDTO.TyLeHoanThanh = congViecDTO.TyLeHoanThanh != 0 ? Math.Round(congViecDTO.TyLeHoanThanh, 1) : 0;
            }

            var congViec = congViecDTO.ToEntity();
            await _congViecService.UpdateCongViec(congViec);
            return Ok(congViec);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}"), Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> DeleteCongViec(int id)
        {
            await _congViecService.DeleteCongViec(id);
            return Ok();
        }
    }
}
