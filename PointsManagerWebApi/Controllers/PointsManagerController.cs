using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PointsManagerWebApi.Entities;
using PointsManagerWebApi.Entities.Models;
using PointsManagerWebApi.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PointsManagerWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointsManagerController : ControllerBase
    {
        private readonly ILogger<PointsManagerController> logger;
        private static User user = new User();

        public PointsManagerController(ILogger<PointsManagerController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("get-balance")]
        public ActionResult GetBalance()
        {
            try
            {
                return Ok(user.GetAllPayersPointBalance());
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return StatusCode(500);
            }
        }

        [HttpPatch]
        [Route("add-transaction")]
        public HttpStatusCode AddTransaction([FromBody] AddPointsRequest addPointsRequest)
        {
            try
            {
                logger.LogInformation(addPointsRequest.Payer);
                user.AddTransaction(addPointsRequest);
                logger.LogInformation(user.RawTransactionList[0].Payer);
                return HttpStatusCode.OK;
            }
            catch(Exception e)
            {
                logger.LogError(e.ToString());
                return HttpStatusCode.InternalServerError;
            }
        }

        [HttpPatch]
        [Route("redeem-points")]
        public ActionResult RedeemPoints([FromBody] SpendRequest spendRequest)
        {
            try
            {
                return Ok(user.RedeemPoints(spendRequest));
            }
            catch(InvalidPointRedemptionRequestException invalidRedemptionRequestException)
            {
                logger.LogError(invalidRedemptionRequestException.ToString());
                return BadRequest(spendRequest);
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return StatusCode(500);
            }

        }
    }
}
