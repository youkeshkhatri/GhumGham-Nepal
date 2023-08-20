using GhumGham_Nepal.Models;
using GhumGham_Nepal.Repository;
using GhumGham_Nepal.Services.GhumGham_Nepal.Services;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.Extensions.Hosting;

namespace GhumGham_Nepal.Services
{
    public class CommonAttachmentService : ICommonAttachmentService
    {

        #region Ctor/Properties

        private readonly ILogger _loggingService;
        private readonly IRepository<CommonAttachment> _commonAttachmentRepository;
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _env;


        /// <summary>
        /// Inject services
        /// </summary>
        /// <param name="loggingService"></param>
        /// <param name="commonAttachmentRepository"></param>
        /// <param name="appSettings"></param>
        /// <param name="localizationService"></param>
        /// <param name="env"></param>
        public CommonAttachmentService
        (
              ILogger<CommonAttachmentService> loggingService,
              IRepository<CommonAttachment> commonAttachmentRepository,
              IOptions<AppSettings> appSettings,
              IWebHostEnvironment env

        )
        {
            _loggingService = loggingService;
            _commonAttachmentRepository = commonAttachmentRepository;
            _appSettings = appSettings.Value;
            _env = env;
        }

        #endregion

        #region protected
        protected void CreateThumbnail(string filename, int desiredWidth, int desiredHeight, string outFilename)
        {
            using (Image img = Image.FromFile(filename))
            {

                float widthRatio = (float)img.Width / (float)desiredWidth;
                float heightRatio = (float)img.Height / (float)desiredHeight;

                float ratio = heightRatio > widthRatio ? heightRatio : widthRatio;
                int newWidth = Convert.ToInt32(Math.Floor((float)img.Width / ratio));
                int newHeight = Convert.ToInt32(Math.Floor((float)img.Height / ratio));

                using (Bitmap dstImg = new Bitmap(newWidth, newHeight))
                {
                    dstImg.SetResolution(300, 300);
                    using (Graphics g = Graphics.FromImage(dstImg))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;

                        // Resize the original
                        g.DrawImage(img, 0, 0, newWidth, newHeight);
                        //g.Dispose();
                    }

                    using (EncoderParameters encoderParameters = new EncoderParameters(1))
                    {
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, 100);          // 100% Percent Compression
                        dstImg.Save(outFilename, ImageCodecInfo.GetImageEncoders()[1], encoderParameters);   // jpg format
                        //dstImg.Dispose();
                    }
                }
            }
        }

        protected bool IsImage(string pExtension)
        {
            if (pExtension.ToLower() == ".jpg" || pExtension.ToLower() == ".gif" || pExtension.ToLower() == ".bmp" || pExtension.ToLower() == ".jpeg" || pExtension.ToLower() == ".png")
                return true;
            else
                return false;
        }
        protected int ConvertBytesIntoKB(int byteValue)
        {
            double mb = (double)byteValue / 1024;
            if (string.IsNullOrEmpty(mb.ToString("#")))
                return 0;
            return Convert.ToInt32(mb.ToString("#"));
        }
        protected string GetSize(int byteValue, int minFileSizeKB, int maxFileSizeInMb, out string msg)
        {
            msg = "";

            if (byteValue < 1024)
            {
                msg = string.Format("Size should be at least {0} KB", minFileSizeKB);
                return "";
            }

            double kb = ConvertBytesIntoKB(byteValue);

            //if (kb < minFileSizeKB)
            //{
            //    msg = string.Format("Size should be at least {0} KB", minFileSizeKB);
            //    return "";
            //}

            if (kb > (1024 * maxFileSizeInMb))
            {
                msg = string.Format("Size cannot be greater than {0} MB", maxFileSizeInMb);
                return "";
            }

            return kb + " kb";
        }


        #endregion

        #region Methods

        /// <summary>
        /// Attachment upload to common table
        /// </summary>
        /// <param name="files">Multiple attachment collection list</param>
        /// <param name="subFolder">Any specific sub-folder name inside upload folder</param>
        public ServiceResult<List<CommonAttachment>> UploadCommonAttachment(List<FileUploadRequest> files, string? subFolder = null, bool isCompanyLogo = false)
        {
            var fname = "";
            string loc = "";
            try
            {
                var attachmentList = new List<CommonAttachment>();

                //if attachment collection is empty 
                if (files.Count == 0)
                {
                    return ServiceResult<List<CommonAttachment>>.Fail("Attachement.NotFound");
                }

                //upload location
                //get from appsetting
                var uploadLocation = GetUploadLocation(isCompanyLogo);

                if (!string.IsNullOrEmpty(subFolder)) uploadLocation += subFolder + "/";
                var fileDir = uploadLocation;

                //if directory not exists, then create it
                if (!Directory.Exists(fileDir))
                    Directory.CreateDirectory(fileDir);

                //each file uploaded status information
                var statusMessage = new List<string>();
                loc = fileDir;

                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    string userFileName = file.FileName;
                    fname = userFileName;
                    string ext = Path.GetExtension(userFileName);

                    //only valid attachment will be allowed to upload
                    if (!IsDocumentValidForUpload(ext))
                    {
                        _loggingService.LogError("Please upload only image or document files, other file type not allowed.");
                        statusMessage.Add($"Attachement.Invalid {ext}");
                        continue;
                    }

                    //TODO:: validate attachment size
                    /*
                    string fileSizeMsg = "";
                    string fileSize = GetSize(file.File.Length, 10, IsImage(ext) ? 2 : 10, out fileSizeMsg); //min:10KB, max:2MB(img), 10MB(pdf)

                    if (string.IsNullOrEmpty(fileSize))
                    {
                        statusMessage.Add(fileSizeMsg + " in " + userFileName);
                        return new ServiceResult<List<CommonAttachment>>(false)
                        {
                            MessageType = MessageType.ValidationFailed,
                            Message = statusMessage,
                            Data = attachmentList
                        };
                    }
                    */


                    string serverFileName = Guid.NewGuid().ToString() + ext;
                    string fullPath = Path.Combine(fileDir, serverFileName);

                    if (UploadFile(file.File, fullPath))
                    {
                        double fileSize = file.File.Length;
                        var thumbnail = "";
                        if (IsImage(ext))
                        {
                            try
                            {
                                string thumbnailLocation = Path.Combine(fileDir, Guid.NewGuid().ToString() + ext);
                                CreateThumbnail(fullPath, 200, 200, thumbnailLocation);
                                thumbnail = Path.GetFileName(thumbnailLocation);
                            }
                            catch (Exception ex)
                            {
                                thumbnail = serverFileName;
                            }
                        }


                        var attachment = new CommonAttachment
                        {
                            FileFormat = ext.Replace(".", "", StringComparison.OrdinalIgnoreCase).Trim(),
                            FileType = file.ContentType,
                            FileLocation = _appSettings.FileRootFolderName,
                            ServerFileName = serverFileName,
                            ServerThumbnailFileName = thumbnail,
                            UserFileName = userFileName,
                            Size = ToSizeString(fileSize)
                        };

                        if (isCompanyLogo)
                            attachment.FileLocation = "LogoBanner/";

                        attachmentList.Add(attachment);

                        statusMessage.Add($"Attachement.Uploaded {userFileName}");
                    }
                    else
                    {
                        statusMessage.Add($"Attachement.Error {userFileName}");
                    }
                }
                _loggingService.LogInformation("File uploaded. ");
                return new ServiceResult<List<CommonAttachment>>(true)
                {
                    Message = statusMessage,
                    Data = attachmentList
                };
            }
            catch (Exception exp)
            {
                string msg = string.Format("filename:{0} path:{1} Mesage:{2}", fname, loc, ("Attachement.Failed"));
                _loggingService.LogError(exp.Message);

                return ServiceResult<List<CommonAttachment>>.Fail(msg);
            }
        }

        /// <summary>
        /// File upload to directory
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected bool UploadFile(byte[] file, string path)
        {
            try
            {
                File.WriteAllBytes(path, file);
                // file.SaveAs(HttpContext.Current.Server.MapPath(relativePath));
                return true;
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Document size with suffix
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected static string ToSizeString(double bytes)
        {
            string[] sizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int mag = (int)Math.Log(bytes, 1024);
            decimal adjustedSize = (decimal)bytes / (1 << (mag * 10));
            return $"{adjustedSize:n1} {sizeSuffixes[mag]}";
        }

        /// <summary>
        /// Check valid document for uploading by its extension
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        protected static bool IsDocumentValidForUpload(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
                return true;

            return fileExtension.ToLower(CultureInfo.CurrentCulture) == ".doc" || fileExtension.ToLower(CultureInfo.CurrentCulture) == ".docx" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".xls" || fileExtension.ToLower(CultureInfo.CurrentCulture) == ".xlsx" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".pdf" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".png" || fileExtension.ToLower(CultureInfo.CurrentCulture) == ".jpeg" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".jpg" || fileExtension.ToLower(CultureInfo.CurrentCulture) == ".bmp" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".gif" || fileExtension.ToLower(CultureInfo.CurrentCulture) == ".mp4" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".avi" || fileExtension.ToLower(CultureInfo.CurrentCulture) == ".mkv" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".ts" || fileExtension.ToLower(CultureInfo.CurrentCulture) == ".mov" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".flv" || fileExtension.ToLower(CultureInfo.CurrentCulture) == ".webm" ||
                   fileExtension.ToLower(CultureInfo.CurrentCulture) == ".wmv";
        }

        /// <summary>
        /// Multiple attachment delete from table
        /// </summary>
        /// <param name="parentTableName"></param>
        /// <param name="parentTablePkId"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteCommonAttachmentAsync(string parentTableName, string parentTablePkId, bool removeFile = false)
        {

            var attachmentList = _commonAttachmentRepository.GetMany(wh => wh.ParentTableName == parentTableName && wh.ParentRecordID == parentTablePkId);

            //if attachment collection is empty, then return 
            var commonAttachments = attachmentList.ToList();
            if (!commonAttachments.Any())
                return ServiceResult.Fail("Attachement.NotFound");


            _commonAttachmentRepository.DeleteRange(commonAttachments.ToList()); //delete attachment from the database table

            await _commonAttachmentRepository.CommitAsync().ConfigureAwait(false);
            //  string uploadLocation = string.IsNullOrEmpty(_appSettings.FileBasePath) ? _env.WebRootPath : _appSettings.FileBasePath;
            if (removeFile)
            {
                var uploadLocation = GetUploadLocation(false);
                foreach (var attachment in commonAttachments)
                {
                    var path = Path.Combine(uploadLocation, attachment.ServerFileName);
                    if (File.Exists(path)) //delete attachment from the directory
                        File.Delete(path);
                }
            }
            return ServiceResult.Success("Attachement.Deleted" + parentTablePkId + " " + parentTableName);

        }

        /// <summary>
        /// Single attachment delete from folder and table
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public async Task<ServiceResult<long>> DeleteCommonAttachmentAsync(long attachmentId)
        {
            try
            {
                var attachment = await _commonAttachmentRepository.GetByIdAsync(attachmentId).ConfigureAwait(false);

                //attachment id validation
                if (attachment == null)
                    return ServiceResult<long>.Fail("DataNotFound", "Id " + attachmentId);
                var path = Path.Combine(Path.Combine(_env.ContentRootPath, attachment.FileLocation), attachment.ServerFileName);
                if (File.Exists(path)) //delete attachment from the directory
                    File.Delete(path);

                _commonAttachmentRepository.Delete(attachment); //delete attachment from the database table
                await _commonAttachmentRepository.CommitAsync().ConfigureAwait(false);

                return ServiceResult<long>.Success("DeleteRecord");
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex.Message);
                return ServiceResult<long>.Fail(ex.Message);
            }
        }

        public IEnumerable<CommonAttachment> GetAllCommonAttachment(string parentTableName, string parentTablePkId)
        {
            return _commonAttachmentRepository.GetMany(wh => wh.ParentTableName == parentTableName && wh.ParentRecordID == parentTablePkId);
        }

        /// <summary>
        /// save attachment list
        /// </summary>
        /// <param name="attachments"></param>
        public async Task<List<CommonAttachment>> SaveAttachmentAsync(List<CommonAttachment> attachments)
        {
            try
            {
                _commonAttachmentRepository.AddRange(attachments);
                await _commonAttachmentRepository.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception ex) { }
            return attachments;
        }

        /// <summary>
        /// save attachment 
        /// </summary>
        /// <param name="attachment"></param>
        public async Task SaveAttachmentAsync(CommonAttachment attachment)
        {
            await _commonAttachmentRepository.AddAsync(attachment).ConfigureAwait(false);
            await _commonAttachmentRepository.CommitAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets common attachment by attachmentId
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public async Task<CommonAttachment> GetByIdAsync(long attachmentId)
        {
            return await _commonAttachmentRepository.GetByIdAsync(attachmentId).ConfigureAwait(false);
        }
        /// <summary>
        /// gets common attachments
        /// </summary>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public async Task<List<CommonAttachment>> GetCommonAttachmentAsync(List<CommonAttachment> attachments)
        {
            var commonAttachments = new List<CommonAttachment>();
            foreach (var item in attachments)
            {
                var attachment = await GetByIdAsync(item.AttachmentID).ConfigureAwait(false);
                if (attachment != null)
                {
                    commonAttachments.Add(attachment);
                }
            }
            return commonAttachments;
        }

        public ServiceResult DeleteFile(List<string> serverFileName)
        {
            string uploadLocation = GetUploadLocation(false, null, false);
            foreach (var file in serverFileName)
            {
                if (!string.IsNullOrEmpty(file))
                {
                    var combinedPath = Path.Combine(uploadLocation, file);
                    File.Delete(combinedPath);
                }
            }

            return ServiceResult<long>.Success("DeleteRecord");

        }

        public string GetUploadLocation(bool isCompanyLogo, string? subFolder = null, bool isUploading = true)
        {
            string uploadLocation = string.IsNullOrEmpty(_appSettings.FileBasePath) ? _env.WebRootPath : _appSettings.FileBasePath;

            if (isUploading)
                _loggingService.LogInformation("Uploading file to " + uploadLocation);
            else
                _loggingService.LogInformation("Removing file from " + uploadLocation);

            if (string.IsNullOrEmpty(_appSettings.FileRootFolderName))
                if (_env.IsDevelopment())
                    uploadLocation = Path.Combine(uploadLocation, "Images/Places/Thumbnail/");
                else
                    throw new Exception(("File.RootFolderNotFound"));
            else if (isCompanyLogo)
                uploadLocation = Path.Combine(uploadLocation, "LogoBanner/");
            else
                uploadLocation = Path.Combine(uploadLocation, _appSettings.FileRootFolderName);

            if (!string.IsNullOrEmpty(subFolder)) uploadLocation += subFolder + "/";

            return uploadLocation;

        }

        #endregion

        public class AppSettings
        {
            public string? FileBasePath { get; set; }
            public string? FileRootFolderName { get; set; }
            public string? FileBaseUrlPrefix
            {
                get
                {
                    return $"/{FileRootFolderName}";
                }
            }
        }

    }

    public static class FileExtension
    {
        public static byte[] ReadBytes(this IFormFile file)
        {
            using (var fileStream = file.OpenReadStream())
            {
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    return fileBytes;
                }
            }
        }

    }
}
