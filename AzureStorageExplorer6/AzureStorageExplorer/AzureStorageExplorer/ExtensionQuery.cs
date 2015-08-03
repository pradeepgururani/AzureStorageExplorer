using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageExplorer
{
  public static class ExtensionQuery
    {
      public static async Task<List<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null) where T : ITableEntity, new()
      {

          var items = new List<T>();
          TableContinuationToken token = null;

          do
          {

              Task<TableQuerySegment<T>> seg = await Task.WhenAny(table.ExecuteQuerySegmentedAsync<T>(query, token));
              token = seg.Result.ContinuationToken;
              items.AddRange(seg.Result);
             
              if (onProgress != null)
              {

                  onProgress(items);
                  //onProgress.Report(items);
              }

          } while (token != null && !ct.IsCancellationRequested);

          return items;
      }


      //public static  List<TableQuerySegment<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null) where T : ITableEntity, new()
      //// public static async Task<List<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, CancellationToken ct = default(CancellationToken), IProgress<IList<T>> onProgress = null) where T : ITableEntity, new()
      //{

      //    var items = new List<TableQuerySegment<T>>();
      //    TableContinuationToken token = null;

      //    do
      //    {

      //        TableQuerySegment<T> seg = Task <TableQuerySegment<T>>.Factory.StartNew(() =>
      //        {
      //           var xyz= table.ExecuteQuerySegmentedAsync<T>(query, token);
                  
      //           //token = xyz.ContinuationToken;
      //           items.Add(xyz.Result);
      //           return items;
      //        });

             

      //    } while (token != null && !ct.IsCancellationRequested);

         
      //}
    }
}
