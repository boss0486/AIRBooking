using AIRService.Models;
using AIRService.WebService.VNA.Authen;
using System;
using System.Threading.Tasks;

namespace AIR.Helper.Session
{
    public class VNA_SessionService : IDisposable
    {
        private readonly TokenModel _tokenModel = null;
        private bool m_Disposed = false;

        public VNA_SessionService(TokenModel tokenModel)
        {
            _tokenModel = tokenModel;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    // các đối tượng có Dispose gọi ở đây
                    VNA_AuthencationService.CloseSession(_tokenModel);
                }
                // giải phóng các tài nguyên không quản lý được cửa lớp
                m_Disposed = true;
            }
        }

        ~VNA_SessionService()
        {
            Dispose(false);
        }
        // *****************************************************************************************************************************************************************************


    }
}
