using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace macaroni_dev.Models
{
    public class GlobalSession
    {
        
        private static GlobalSession? instance;
        public int? CurrentUserID { get; set; }
        private GlobalSession() 
        { 

        }
        private static GlobalSession Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GlobalSession();
                }
                return instance;
            }
        }
        public void SetUserId(int userId)
        {
            CurrentUserID = userId;
        }
        public void ClearSession()
        {
            if (CurrentUserID != null)
            {
                CurrentUserID = null;
            }
        }
    }
}
