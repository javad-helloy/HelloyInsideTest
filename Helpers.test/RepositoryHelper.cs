using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InsideModel.Models;
using InsideModel.repositories;

namespace Helpers.test
{
    public static class RepositoryHelper
    {
        public static IRepository<Client> RepositoryLocalWithOneClient
        {
            get
            {
                var client = ModelHelper.TestClient1AllDataNoReferences;
                var repository = new LocalRepository<Client>();
                repository.Add(client);

                return repository;
            }
        }
    }
}
