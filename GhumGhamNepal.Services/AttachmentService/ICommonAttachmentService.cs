using GhumGham_Nepal.Services;
using GhumGham_Nepal.Services.GhumGham_Nepal.Services;
using GhumGhamNepal.Core.Models.DbEntity;

namespace GhumGhamNepal.Core.Services.AttachmentService
{
    public interface ICommonAttachmentService
    {
        string GetUploadLocation(bool isCompanyLogo, string subFolder = null, bool isUploading = true);
        //string GetFilePath(bool isCompanyLogo, string serverFillename);
        //string GetFilePath(bool isCompanyLogo);
        ServiceResult<List<CommonAttachment>> UploadCommonAttachment(List<FileUploadRequest> files, string subFolder = null, bool isCompanyLogo = false);
        Task<ServiceResult> DeleteCommonAttachmentAsync(string parentTableName, string parentTablePkID, bool removeFile = false);
        Task<ServiceResult<long>> DeleteCommonAttachmentAsync(long attachmentId);

        IEnumerable<CommonAttachment> GetAllCommonAttachment(string parentTableName, string parentTablePkID);

        Task<List<CommonAttachment>> GetCommonAttachmentAsync(List<CommonAttachment> attachments);
        Task<List<CommonAttachment>> SaveAttachmentAsync(List<CommonAttachment> attachments);
        Task SaveAttachmentAsync(CommonAttachment attachment);
        Task<CommonAttachment> GetByIdAsync(long attachmentId);
        ServiceResult DeleteFile(List<string> serverfileName);
    }
}
