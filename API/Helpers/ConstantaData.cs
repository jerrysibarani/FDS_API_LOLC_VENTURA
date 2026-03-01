namespace API.Helpers
{
    public class ConstantaData
    {
        public static string INTERNAL = "INTERNAL";
        public static string EXTERNAL = "EXTERNAL";
        public static string SUPERADMIN = "SUPERADMIN";

        public static string PAID = "PAID";
        public static string CANCEL = "CANCEL";
        public static string UNPAID = "UNPAID";

        public static string ONPROGRESS = "ON PROGRESS";
        public static string NOPAY = "NO PAY";
        public static string ALL = "ALL";
        public static string COLUMN_GENERAL = "GENERAL";
        public static string EXTENTION_IMPORT = ".csv,.xls,.xlsx";
        public static string EXTENTION_FILE = ".pdf,.jpg,.jpeg,.bmp,.tif,.png,.doc,.docx";
        public static string DOCTYPECERTIFICATE = "CERTIFICATE";
        public static string DOCTYPEWARKAH = "WARKAH";
        public static string DOCTYPEAKTA = "AKTA";
        public static string DOCTYPEPNBP = "PNBP";
        public static string DOCTYPESALINAN = "SALINAN";
        public static string DOCTYPEMINUTA = "MINUTA";

        public static string RoleAdministrator = "Administrator";
        public static string RoleSuperAdmin = "SuperAdmin";
        public static List<int> PagesSized()
        {
            List<int> listCatg = new()
            {
                {10},
                {20},
                {25},
                {50},
                {100}
            };
            return listCatg;
        }

        public static Dictionary<int, string> CategoriUsers()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Email"},
                {2,"Username"},
                {3,"Full Name"},
                {4,"Address"},
                {5,"City"},
                {6,"Branch Name"}
            };
            return listCatg;
        }

        public static Dictionary<int, string> CategoriUsersClient()
        {
            Dictionary<int, string> listCatg = CategoriUsers();
            listCatg.Add(7, "Customer Type");
            listCatg.Add(8, "Company Name");
            return listCatg;
        }

        public static Dictionary<int, string> CategoriUsersSA()
        {
            Dictionary<int, string> listCatg = CategoriUsers();
            listCatg.Add(7, "Customer Type");
            listCatg.Add(8, "Company Name");
            listCatg.Add(9, "Client Code");
            return listCatg;
        }
        public static Dictionary<int, string> CategoriClient()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Client Code"},
                {2,"Client Name"},
                {3,"Telp / HP"},
                {4,"Email"},
                {5,"Kota"},
            };
            return listCatg;
        }



        public static Dictionary<int, string> CategoriCustomers()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Customer Code"},
                {2,"Customer Name"},
                {3,"Type Name"},
                {4,"SK"},
                {5,"Kota"}
            };
            return listCatg;
        }

        public static Dictionary<int, string> CategoriBranches()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Branch Code"},
                {2,"Branch Name"}
            };
            return listCatg;
        }
        public static Dictionary<int, string> CategoriBranchesClient()
        {
            Dictionary<int, string> listCatg = CategoriBranches();
            listCatg.Add(3, "Company Code");
            listCatg.Add(4, "Company Name");
            return listCatg;
        }

        public static Dictionary<int, string> CategoriBranchesSA()
        {
            Dictionary<int, string> listCatg = CategoriBranches();
            listCatg.Add(3, "Company Code");
            listCatg.Add(4, "Company Name");
            listCatg.Add(5, "Client Code");
            listCatg.Add(6, "Client Name");
            return listCatg;
        }

        public static Dictionary<int, string> CategoriAhu()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Username Ahu"}
            };
            return listCatg;
        }
        public static Dictionary<int, string> CategoriAhuClient()
        {
            Dictionary<int, string> listCatg = CategoriAhu();
            listCatg.Add(2, "Company Code");
            listCatg.Add(3, "Company Name");
            return listCatg;
        }
        public static Dictionary<int, string> CategoriAhuSA()
        {
            Dictionary<int, string> listCatg = CategoriAhu();
            listCatg.Add(2, "Company Code");
            listCatg.Add(3, "Company Name");
            listCatg.Add(4, "Client Code");
            return listCatg;
        }

        public static Dictionary<int, string> CategoriRoleUsers()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Role Name"},
                {2,"Username"},
                {3,"Email"},
                {4,"Full Name"},
                {5,"Branch Code"}
            };
            return listCatg;
        }

        public static Dictionary<int, string> CategoriRoleUsersClient()
        {
            Dictionary<int, string> listCatg = CategoriRoleUsers();
            listCatg.Add(6, "Customer Code");
            listCatg.Add(7, "User Type");
            return listCatg;
        }

        public static Dictionary<int, string> CategoriRoleUsersSA()
        {
            Dictionary<int, string> listCatg = CategoriRoleUsers();
            listCatg.Add(6, "Customer Code");
            listCatg.Add(7, "User Type");
            listCatg.Add(8, "Client Code");
            return listCatg;
        }


        public static Dictionary<int, string> CategoriNotaris()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Nama Notaris"},
                {2,"SK"},
                {3,"Kota"},
                {4,"Provinsi"}
            };
            return listCatg;
        }
        public static Dictionary<int, string> CategoriNotarisClient()
        {
            Dictionary<int, string> listCatg = CategoriNotaris();
            return listCatg;
        }
        public static Dictionary<int, string> CategoriNotarisSA()
        {
            Dictionary<int, string> listCatg = CategoriNotaris();
            listCatg.Add(5, "Client Code");
            listCatg.Add(6, "Client Name");
            return listCatg;
        }


        public static Dictionary<int, string> CategoriAccess()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Access Code"},
                {2,"Access Name"}
            };
            return listCatg;
        }

        public static Dictionary<int, string> CategoriAccessRole()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Role Name"},
                {2,"Access Code"},
                {3,"Access Name"}
            };
            return listCatg;
        }


        public static Dictionary<int, string> CategoriPostParameter()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Post Name"},
                {2,"Post Value"},
                {3,"Post Type"},
                {4,"Post Group"}
            };
            return listCatg;
        }




        public static Dictionary<int, string> CategoriInvoiceStatus()
        {
            Dictionary<int, string> listCatg = new()
            {
                {0,"On Progress"},
                {1,"Paid"},
                {2,"Unpaid"},                
                {3,"Cancel"}
            };
            return listCatg;
        }

        public static Dictionary<int, string> CategoriInvoiceStatusClient()
        {
            Dictionary<int, string> listCatg = CategoriInvoiceStatus();
            listCatg.Add(6, "Company Code");
            listCatg.Add(7, "Company Name");
            return listCatg;
        }

        public static Dictionary<int, string> CategoriInvoiceStatusSA()
        {
            Dictionary<int, string> listCatg = CategoriInvoiceStatus();
            listCatg.Add(6, "Company Code");
            listCatg.Add(7, "Company Name");
            listCatg.Add(8, "Client Code");
            listCatg.Add(9, "Client Name");
            return listCatg;
        }


        public static Dictionary<int, string> CategoriConfig()
        {
            Dictionary<int, string> listCatg = new()
            {
                {1,"Client Code"},
                {2,"Client Name"},
                {3,"Customer Code"},
                {4,"Customer Name"}
            };
            return listCatg;
        }

    }
}
