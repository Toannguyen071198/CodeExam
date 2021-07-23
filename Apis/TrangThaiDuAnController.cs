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
    public class TrangThaiDuAnController : BaseApiController
    {
        private readonly ITrangThaiDuAnService _trangThaiDuAnService;
        private readonly ICongViecService _congViecService;
        private readonly IChiTietCongViecService _chiTietCongViecService;
        public TrangThaiDuAnController(ITrangThaiDuAnService trangThaiDuAnService
            , ICongViecService congViecService, IChiTietCongViecService chiTietCongViecService)
        {
            _trangThaiDuAnService = trangThaiDuAnService;
            _congViecService = congViecService;
            _chiTietCongViecService = chiTietCongViecService;
        }

        [ProducesResponseType(typeof(PagedResult<TrangThaiDuAnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, Authorize]
        public async Task<IActionResult> GetTrangThaiDuAnByDuAnId(
            [FromQuery] int duAnId,
            [FromQuery] string tenCongViec = null,
            [FromQuery] int? loaiCongViecId = null,
            [FromQuery] DateTime? tuNgay = null,
            [FromQuery] DateTime? denNgay = null,
            [FromQuery] int? tagId = null,
            [FromQuery] int? nguoiLamId = null,
            [FromQuery] Pagination pagination = null)
        {
            var query = _trangThaiDuAnService.GetTrangThaiDuAnByDuAnId(duAnId);
            var trangThaiDuAns = PagedList.Create(query, pagination.Page - 1, pagination.ItemsPerPage);
            pagination.TotalItems = trangThaiDuAns.TotalCount;

            var trangThaiDuAnDTOs = trangThaiDuAns.Select(TrangThaiDuAnDTO.FromEntity).ToList();
            foreach (var trangThai in trangThaiDuAnDTOs)
            {
                var congViecDTOs = _congViecService.GetCongViec(tenCongViec,
                                                                trangThai.DuAnId,
                                                                trangThai.TrangThaiId,
                                                                loaiCongViecId,
                                                                tuNgay,
                                                                denNgay,
                                                                tagId,
                                                                nguoiLamId)
                                                    .Select(CongViecDTO.FromEntity)
                                                    .ToList();
                foreach (var congViec in congViecDTOs)
                {
                    congViec.SoLuongChiTietCongViec = await _chiTietCongViecService.GetSoLuongChiTietCongViecByCongViecId(congViec.Id);
                    congViec.SoLuongChiTietCongViecHoanThanh = await _chiTietCongViecService
                        .GetSoLuongChiTietCongViecByCongViecIdAndTrangThaiId(congViec.Id, 4);
                }
                trangThai.TrangThai.CongViec = congViecDTOs;
            }
            var result = new PagedResult<TrangThaiDuAnDTO>(pagination, trangThaiDuAnDTOs);
            return Ok(result);
        }

        [ProducesResponseType(typeof(PagedResult<TrangThaiDuAnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("duan/{duAnId}"), Authorize]
        public async Task<IActionResult> GetTrangThaiByDuAnId(
            int duAnId,
            [FromQuery] Pagination pagination = null)
        {
            var query = _trangThaiDuAnService.GetTrangThaiDuAnByDuAnId(duAnId);
            var trangThaiDuAn = PagedList.Create(query, pagination.Page - 1, pagination.ItemsPerPage);
            pagination.TotalItems = trangThaiDuAn.TotalCount;
            var result = new PagedResult<TrangThaiDuAnDTO>(pagination, trangThaiDuAn.Select(TrangThaiDuAnDTO.FromEntity));
            return Ok(result);
        }
        [ProducesResponseType(typeof(TrangThaiDuAnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> CreateTrangThaiDuAn(TrangThaiDuAnDTO trangThaiDuAnDTO)
        {
            var trangThaiDuAn = trangThaiDuAnDTO.ToEntity();
            var seviceResult = await _trangThaiDuAnService.CreateTrangThaiDuAn(trangThaiDuAn);
            if (!seviceResult.Succeeded)
                return BadRequest(seviceResult.Errors);
            return Ok(trangThaiDuAnDTO);
        }

        [ProducesResponseType(typeof(TrangThaiDuAnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}"), Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> UpdateTrangThaiDuAn(int id, [FromBody] TrangThaiDuAnDTO trangThaiDuAnDTO)
        {
            var trangThaiDuAn = trangThaiDuAnDTO.ToEntity();
            var seviceResult = await _trangThaiDuAnService.UpdateTrangThaiDuAn(trangThaiDuAn);
            if (!seviceResult.Succeeded)
                return BadRequest(seviceResult.Errors);
            return Ok(trangThaiDuAnDTO);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}"), Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> DeleteTrangThaiDuAn(int id)
        {
            await _trangThaiDuAnService.DeleteTrangThaiDuAn(id);
            return Ok();
        }
    }
}
