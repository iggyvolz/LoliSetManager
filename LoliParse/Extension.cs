using Parse;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoliParse
{
    static class Extensions
    {
        public static async Task SaveOrUpdateAsync(this ParseObject parseObject, string uniqueColumn)
        {
            try
            {
                await parseObject.SaveAsync();
            }
            catch (ParseException pe)
            {
                if (pe.Code != ParseException.ErrorCode.DuplicateValue)
                {
                    throw pe; // nah we can't take this
                }
                // Ah, we need to update the existing one
                ParseQuery<ParseObject> query = ParseObject.GetQuery(parseObject.ClassName).WhereEqualTo(uniqueColumn, (string)parseObject[uniqueColumn]);
                ParseObject newObject = await query.FirstAsync(); // TODO what if we deleted it since?
                foreach (string key in parseObject.Keys)
                {
                    if (key != "Set") newObject[key] = parseObject[key];
                }
                await newObject.SaveAsync();
            }
        }
    }
}
