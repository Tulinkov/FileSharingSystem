using Microsoft.AspNetCore.Mvc;

using Authentication;
using SborkaDrive.Models;
using SborkaDrive.Data;
using Microsoft.EntityFrameworkCore;

namespace SborkaDrive.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IConfiguration _config;

        //constructor
        public FilesController(ApiContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>
        /// Adds a file to the upload list
        /// </summary>
        /// <param name="orderNumber">Order number whose file is to be uploaded</param>
        /// <returns>Information about the file to upload and the result of the operation</returns>
        [HttpPost]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        public async Task<JsonResult> One(int orderNumber)
        {
            return Many(new int[] { orderNumber }).Result;
        }

        /// <summary>
        /// Adds many files to the upload list
        /// </summary>
        /// <param name="orderNumbers">List of order numbers whose files are to be uploaded</param>
        /// <returns>Information about the files to upload and the result of the operation</returns>
        [HttpPost]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        public async Task<JsonResult> Many(IEnumerable<int> orderNumbers)
        {
            List<LayoutFile> files = new List<LayoutFile>();
            foreach (int nb in orderNumbers)
            {
                //Check if the file information is already in the database
                var existedFile = _context.Files
                    .Where(f => f.OrderId == nb)
                    .FirstOrDefault();
                if (existedFile != null)
                {
                    files.Add(existedFile);
                    continue;
                }
                //Add the file information to database
                LayoutFile? file = null;
                if ((file = await FindFile(nb)) is not null)
                {
                    _context.Files.Add(file);
                    files.Add(file);
                }
            }
            await _context.SaveChangesAsync();
            return (files.Count > 0) ?
                new JsonResult(Ok(files)) :
                new JsonResult(NotFound(orderNumbers));
        }

        /// <summary>
        /// Gets next file information
        /// </summary>
        /// <param name="toDrive">true - to upload; false - to download</param>
        /// <returns>Information about the file that is next in the upload list and the result of the operation</returns>
        [HttpGet]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        public async Task<JsonResult> Next(bool toDrive)
        {
            var res = await _context.Files
                            .Where(f => (f.GoogleFileId == null) == toDrive)
                            .OrderBy(f => f.Id)
                            .FirstOrDefaultAsync();

            return (res is not null) ?
                new JsonResult(Ok(res)) :
                new JsonResult(NotFound(null));
        }

        /// <summary>
        /// Updates the information about Google file id
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <param name="googleFileId">Google file id</param>
        /// <returns>Updated information about the file and the result of the operation</returns>
        //Add Google id of uploaded file
        [HttpPut]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        public async Task<JsonResult> Uploaded(int orderId, string googleFileId)
        {
            var res = await _context.Files
                .Where(f => f.OrderId == orderId)
                .FirstOrDefaultAsync();

            if (res is null)
                return new JsonResult(NotFound(orderId));

            res.GoogleFileId = googleFileId;
            _context.Update(res);
            await _context.SaveChangesAsync();
            return new JsonResult(Ok(res));
        }

        /// <summary>
        /// Deletes the information about the file
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns>Information about the removed file and the result of the operation</returns>
        [HttpDelete]
        [ActionName("One")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        public async Task<JsonResult> DeleteOne(int orderNumber)
        {
            var res = await _context.Files
                .Where (f => f.OrderId == orderNumber)
                .FirstOrDefaultAsync();

            if (res is null)
                return new JsonResult(NotFound(orderNumber));

            _context.Remove(res);
            await _context.SaveChangesAsync();
            return new JsonResult(Ok(res));
        }

        /// <summary>
        /// Finds a file
        /// </summary>
        /// <param name="orderNumber">Number of order</param>
        /// <returns>Information about the found file</returns>
        private async Task<LayoutFile?> FindFile(int orderNumber)
        {
            string pathFolder = _config.GetValue<string>("FileStorage");
            string filePattern = $"*-{orderNumber})*.*";
            try
            {
                string[] dirs = await Task.Run(() =>
                    Directory.GetFiles(pathFolder, filePattern));
                return (dirs.Length > 0) ?
                    new LayoutFile(orderNumber, dirs[0]) :
                    null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
