using FluentResults;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.ImExPort.helper
{
    internal static class StringHelper
    {
        internal static Result<Int16> ConvertStringToInt16(String param)
        {
            try
            {
                return Result.Ok<Int16>(Convert.ToInt16(param));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex);
                return Result.Fail(ex.Message);
            }
        }

        internal static Result<List<Int16>> ConvertStringToListOfInt16(String param)
        {
            try
            {
                if (param.Contains(' '))
                {
                    bool success = true;
                    string[] elementList = param.Split(' ');
                    List<Int16> retval = new List<Int16>(elementList.Length);
                    foreach (String element in elementList)
                    {
                        Result<Int16> toAdd = ConvertStringToInt16(element);
                        if (toAdd.IsSuccess)
                        {
                            retval.Add(toAdd.Value);
                        }
                        else
                        {
                            Log.Logger.Debug("Could not Convert {0} to Int16", element);
                            success = false;
                        }
                    }

                    if (success)
                    {
                        return Result.Ok<List<Int16>>(retval);
                    }
                    else
                    {
                        return Result.Fail(String.Empty);
                    }
                }
                else 
                {
                    //  TODO: Prio 1 - Check functionality as soon new FileDBReader Version is integrated
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex);
                return Result.Fail(ex.Message);
            }
        }
    }
}
