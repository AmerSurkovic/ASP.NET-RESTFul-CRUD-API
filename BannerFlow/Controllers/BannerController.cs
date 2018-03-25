﻿using BannerFlow.Models;
using BannerFlow.Repositories;
using BannerFlow.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace BannerFlow.Controllers
{

    [RoutePrefix("api/banners")]
    public class BannerController : ApiController
    {
        private static BannerService bannerService;

        public BannerController()
        {
            bannerService = new BannerService();
        }

        // GET api/banners
        [Route("")]
        [HttpGet]
        public IEnumerable<BannerDetailDTO> Get()
        {
            try
            {
                return bannerService.GetAll();
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        // GET api/banners/{id}
        [Route("{id:int}")]
        [HttpGet]
        public BannerDetailDTO Get(int id)
        {
            try
            {
                var banner = bannerService.Get(id);
                return banner;
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            
        }

        // POST api/banners
        [Route("")]
        [HttpPost]
        public HttpResponseMessage Post(BannerDTO value)
        {
            HttpResponseMessage result = null;

            if (ModelState.IsValid)
            {
                try
                {
                    var banner = bannerService.Add(value);
                    result = Request.CreateResponse<Banner>(HttpStatusCode.OK, banner);
                    string newItemURL = Url.Link("BannerApi", new { id = banner.Id });
                    result.Headers.Location = new Uri(newItemURL);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message, ex);
                    result = Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
            else
            {
                result = Request.CreateResponse<string>(HttpStatusCode.BadRequest, "State not valid!");
            }
            return result;
        }


        // PUT api/banners/id
        [Route("{id:int}")]
        [HttpPut]
        public HttpResponseMessage Put(int id, BannerDTO value)
        {
            HttpResponseMessage result = null;

            if (ModelState.IsValid)
            {
                try
                {
                    Banner update = bannerService.Put(id, value);
                    result = Request.CreateResponse(HttpStatusCode.Accepted, update);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message, ex);
                    result = Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
            else
            {
                result = Request.CreateResponse<string>(HttpStatusCode.BadRequest, "State not valid!");
            }
            return result;

        }

        // DELETE api/banners/id
        [Route("{id:int}")]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage result = null;

            try
            {
                bannerService.Delete(id);
                result = Request.CreateResponse(HttpStatusCode.Accepted, "Entry was deleted.");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message, ex);
                result = Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        // GET api/banners/{id}/html
        [Route("{id:int}/html")]
        public HttpResponseMessage GetHtml(int id)
        {
            try { 
                var banner = bannerService.GetHtml(id);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(banner.Html);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                return response;
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        /*
         * This implementation of <GetHtml> returns HTML parameter of a Banner object through JSON.
         * I opted for the other one because it is easier to load the HTML immediately. 
        public BannerDTO GetHtml(int id)
        {
            var banner = bannerService.GetHtml(id);
            if (banner == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return banner;
        }*/
    }
}
