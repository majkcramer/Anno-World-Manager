using Anno_World_Manager.ImExPort2.model;
using Anno_World_Manager.model;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.ImExPort2
{
    internal static class Service
    {
        /// <summary>
        /// Gets a Session from a Vanilla Map Template
        /// </summary>
        /// <param name="mapPath"Anno internal Path to a a7tinfo></param>
        /// <returns></returns>
        public static Result<Session> GetSessionFromVanillaMapTemplate(string mapPath)
        {
            Task<Result<a7tinfoModel>> generator = Task.Run(() => GenerateAsync(mapPath));
            Result<a7tinfoModel> t = generator.Result;
            if (t.IsFailed) { return Result.Fail(String.Empty); }
            return ConvertA7tinfo(t.Value);
        }


        public static async Task<Result<a7tinfoModel>> GenerateAsync(String path)
        {
            var stream = Runtime.Anno1800GameData.DataArchive.OpenRead(path);
            if (stream == null)
            {
                Log.Logger.Error("Could not read from Vanilla DataStream: {0}", path);
                return Result.Fail(String.Empty);
            }

            Result<a7tinfoModel> a7tinfodata = await Anno_World_Manager.ImExPort2.Reader.A7tinfoModelFromA7tinfoAsync(stream, path); 
            
            return a7tinfodata;
        }

        /// <summary>
        /// Convert a7Tinfo Model to a Session
        /// </summary>
        /// <remarks>
        /// For Conversion implementation <see cref="Session.AdoptPropertiesFromA7tinfo(a7tinfoModel)"/>
        /// </remarks>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Session ConvertA7tinfo(a7tinfoModel template)
        {
            return new Session(template);
        }
    }
}
