using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceFinding.Managing_Data.ODBC_Readers
{

    public enum ODBCPriceListTypes
    {
        plFixed = 0,             //Fixed discounted price i.e. List=100.00 Fixed=60.00 
        plStandard = 1,          //Uses standard stock record sales price  
        plDecreasePercent = 2,   //Decrease product sales price by a percentage  
        plDecreaseValue = 3,     //Decrease product sales price by a specified amount 
        plMarkupCost = 4,        //Increase cost price by a percentage 
        plMarkupValue = 5,       //Increase cost price by a specified amount 
        plMarkupSales = 6        //Increase sales price by a percentage

    }//Enum
}//NS
