﻿using CentralDeErros.Core;
using CentralDeErros.Model.Models;
using CentralDeErros.Services.Base;
using CentralDeErros.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CentralDeErros.Services
{
    public class ErrorService : ServiceBase<Error>, IErrorService
    {
        public ErrorService(CentralDeErrosDbContext context) : base(context)
        {
        }

        /// <summary>
        /// List values ​​based on pagination
        /// Can pass the start and end of occurences
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ICollection<Error> List(int? start, int? end, bool archived)
        {
            var response = Context
                                .Errors
                                .Where(x => x.IsArchived == archived)
                                .Skip(start ?? 0);

            if(end.HasValue)
            {
                return response.Take(end.Value).ToList();
            }

            return response.ToList();
                
        }

        /// <summary>
        /// Search based on Environment Id
        /// </summary>
        /// <param name="enviromentId"></param>
        /// <returns></returns>
        public ICollection<Error> SearchByEnviroment(int enviromentId)
        {
            return List(x => x.EnviromentId == enviromentId).ToList();
        }


        /// <summary>
        /// Search based on Error Level
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <returns></returns>
        public ICollection<Error> SearchByErrorLevel(string errorLevel)
        {
            return List(x => x.Level.Name == errorLevel.ToLower())
                .ToList();
        }


        /// <summary>
        /// Search between a given DateTime
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ICollection<Error> SearchByDate(DateTime start, DateTime end)
        {
            return List(x => x.ErrorDate >= start && x.ErrorDate <= end).ToList();
        }

        public new Error Register(Error entry)
        {
            if (!CheckId<Level>(entry.LevelId))
                throw new Exception("LevelId not found");

            if (!CheckId<Microsservice>(entry.MicrosserviceClientId))
                throw new Exception("MicrosserviceClientId not found");

            if (!CheckId<Model.Models.Environment>(entry.EnviromentId))
                throw new Exception("EnviromentId not found");
            
            entry.IsArchived = false;
            
            Context.Add(entry);
            Context.SaveChanges();

            return entry;
        }

        public List<int> ArchiveById(List<int> errorIdList)
        {
            var failed = new List<int>();
            foreach (var errorId in errorIdList)
            {
                if (!CheckId<Error>(errorId))
                {
                    failed.Add(errorId);
                }
                else
                {
                    var error = Fetch(errorId);
                    error.IsArchived = true;
                    Update(error);
                }
            }
            return failed;
        }

        public bool CheckId<T>(int id) where T : class
        {
            return Context.Set<T>().Find(id) != null;
        }

        public bool CheckId<T>(Guid clientId) where T : class
        {
            return Context.Set<T>().Find(clientId) != null;
        }
    }
}
