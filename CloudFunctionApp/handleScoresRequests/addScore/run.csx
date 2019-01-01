#r "System.Data"
#r "Newtonsoft.Json"

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;
public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    //==========================
    //      READ INPUT
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    string bname = data.name;
    int bscore = data.score;
    string btime = data.time;
    //==========================
    //     INPUT CHECK
    if(bname == null){
        return new BadRequestObjectResult("Please supply a name"); // returns 400 response code 
    }
    if(bscore == null){
        return new BadRequestObjectResult("Please supply a score"); // returns 400 response code 
    }
    if(btime == null){

        return new BadRequestObjectResult("Please supply a time"); // returns 400 response code 
    }
    //=========================
    //   PUT DATA IN DB
    var cnnString ="Server=tcp:spinit.database.windows.net,1433;Initial Catalog=Spin-it-Data;Persist Security Info=False;User ID=spinit;Password=SI2018ar;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";//"THE CONNECTION STRING OF YOU SERVER";
    var jsonResult = new StringBuilder();

    using (SqlConnection conn = new SqlConnection(cnnString) )
    {
        conn.Open();
        var sqlQuery = "INSERT INTO Scores(Name,Score,Time) VALUES ('"+bname+"',"+bscore+", '"+btime+"');";
     
        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
        {
            using (SqlDataReader reader = cmd.ExecuteReader()) {
                if (reader.HasRows){
                    while (reader.Read()){
                         jsonResult.Append(reader.GetValue(0).ToString());
                    }
                }
                else{
                    jsonResult.Append("OK");
                }
            }
        }
    }
    return new OkObjectResult(jsonResult);


}