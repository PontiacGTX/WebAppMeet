﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Models;

namespace SharedProject.Factory
{
    public class Factory
    {
        public static  Response GetResponse<TR>(object data,int statusCode =200, bool success=true,params string[] messages)
            
        {
            Type t = typeof(TR);
            if (t ==  typeof(Response))
            {
                if(messages.FirstOrDefault() ==null)
                return new Response { Data = data , StatusCode = statusCode, Success = success };
                

                return new Response { Data = data, Message = messages.FirstOrDefault(), StatusCode = statusCode, Success = success };
            }
            else if (t == typeof(ErrorServerResponse))
            {
                if (messages.FirstOrDefault() == null)
                    return new ErrorServerResponse { Data = null, Message = Factory.GetStringResponse(StringResponseEnum.InternalServerError), StatusCode = statusCode, Success = success };

                return new ErrorServerResponse { Data = null, Message = messages.FirstOrDefault(), StatusCode = 500, Success = success, Validation = messages[1..(messages.Length-1)] };
            }
                return default;
        }

        public static string GetStringResponse(StringResponseEnum error, string valueInString = null!) =>
        error switch
        {
            StringResponseEnum.FailedRequest => "An error occurred while doing a request",
            StringResponseEnum.NotFound => $"Couldn't find any entry of {valueInString} the requested value",
            StringResponseEnum.InternalServerError => "There was an internal server error",
            StringResponseEnum.BadRequestError => $"The model sent was invalid due to one or more validations errors {valueInString}",
            StringResponseEnum.Unathorized => $"The user is not authorized to do request {valueInString}",
            StringResponseEnum.AlreadyInUse => $"The {valueInString} is already in use",
            StringResponseEnum.TimeOut =>$"Request timeout",
            _ => "An unknown/unexpected error happened"

        };
    }

}
