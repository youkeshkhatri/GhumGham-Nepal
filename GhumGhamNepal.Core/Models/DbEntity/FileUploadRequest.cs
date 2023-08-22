namespace GhumGhamNepal.Core.Models.DbEntity
{
    public class FileUploadRequest
    {
        //public string Directory { get; set; }

        /// <summary>
        /// byte array
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public FileUploadRequest(byte[] byteArray, string contentType, string fileName)
        {
            File = byteArray;
            ContentType = contentType;
            FileName = fileName;
        }

    }
}
