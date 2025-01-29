
namespace MyPortalStudent.Domain
{
    public class ApiResult<T>
    {
        public ApiResult()
        {
            this.Success = true;
            this.Code = "PS-EVAL";
            this.Message = "Se procesó la información correctamente";
        }
        /// <summary>
        /// TRUE if the Api attempt is successful, FALSE otherwise.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Api return result code
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// Api return result message
        /// </summary>
        public string Message { get; set; } = null!;
        /// <summary>
        /// Return the data for Api
        /// </summary>
        public T? Data { get; set; }
    }
}
