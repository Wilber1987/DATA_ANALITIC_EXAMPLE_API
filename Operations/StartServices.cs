using BusinessLogic.Connection;
using Operations.Category;
using Operations.DataGenerator;
using Operations.Time;

namespace Operations;

public class StartServices
{
    public async Task<bool> StartServicesApp()
    {
        try
        {
            Console.Write("############### BEGINNN");
            new BDConnection().InitMainConnection();
            /*new CategoryOperation().Excute();
            new TimeOperation().Excute();
            
            DateOLAPOperation.UpdateLastUpdateDate(DateTime.Now);*/
            await SyntheticDataGenerator.Start();
            Console.Write("############### END");
            return true;
        }
        catch (System.Exception ex)
        {
            Console.Write(ex.Message);
            throw;
        }
    }

}
