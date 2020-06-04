﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWalletApi.DTO;
using TestWalletApi.Models;
using TestWalletApi.Services;

namespace TestWalletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IIdempotentService _idempotentService;

        public WalletsController(IWalletService walletService, IIdempotentService idempotentService)
        {
            _walletService = walletService;
            _idempotentService = idempotentService;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Wallet>> GetWallet(long id)
        {
            var wallet = await _walletService.GetWallet(id);

            if (wallet == null)
            {
                return NotFound();
            }

            return wallet;
        }
        
        
        [HttpPost]
        public async Task<ActionResult<Wallet>> PostWallet([FromForm] CreatingWallet wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Parameters is invalid");
            }
            
            try
            {
                var createdWallet = await _walletService.CreateWallet(wallet.UserId);
                return CreatedAtAction("GetWallet", new { id = createdWallet.Id }, createdWallet);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }


        [HttpPut]
        public async Task<IActionResult> PutMoney([FromHeader(Name = "idempotent_key")] string idempotentKey, [FromForm] ChangeMoneyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Parameters is invalid");
            }

            var isFirstRequest = await _idempotentService.CheckIdempotent(idempotentKey, dto);
            if (!isFirstRequest)
            {
                return StatusCode(StatusCodes.Status423Locked);
            }

            try
            {
                var сhangedTab = await _walletService.AddMoney(dto);
                return Ok(сhangedTab);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("transfer")]
        public async Task<IActionResult> TransferMoney([FromHeader(Name = "idempotent_key")] string idempotentKey, [FromForm] ConvertMoneyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Parameters is invalid");
            }

            var isFirstRequest = await _idempotentService.CheckIdempotent(idempotentKey, dto);
            if (!isFirstRequest)
            {
                return StatusCode(StatusCodes.Status423Locked);
            }
            
            try
            {
                var сhangedTab = await _walletService.TransferMoney(dto);
                return Ok(сhangedTab);
            }
            catch (ArgumentException)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}