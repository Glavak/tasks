﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace EvalTask
{
	class EvalProgram
	{
		static void Main(string[] args)
		{
			string input = Console.In.ReadLine();
		    string json = Console.In.ReadToEnd();
            
		    string output;

		    try
		    {
                output = Process(input, JObject.Parse(json));
            }
		    catch (Exception e)
		    {
		        output = Process(input);
		    }

			Console.WriteLine(output);
        }

        public static string Process(string input, JObject constants)
        {
            foreach (var constant in constants)
            {
                input = input.Replace(constant.Key.ToString(), constant.Value.ToString());
            }

            return Process(input);
        }

        public static string Process(string input)
        {
            try
            {
                var result = Evaluate(input);

                if (result == double.PositiveInfinity ||
                    result == double.NegativeInfinity ||
                    double.IsNaN(result))
                    return "error";

                return result.ToString(CultureInfo.InvariantCulture);
            }
            catch (SyntaxErrorException e)
            {
                return "error";
            }
        }


        public static double Evaluate(string expression)
        {
            DataTable table = new DataTable();
            try
            {
                table.Columns.Add("expression", typeof(string), expression);
            }
            catch (Exception e)
            {
                return Double.NaN;
            }
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }
    }

    [TestFixture]
    public class EvalProgram_Should
    {
        [TestCase("1+1", Result = "2")]
        [TestCase("(2 + 2)*2", Result = "8")]
        [TestCase("-2-2", Result = "-4")]
        [TestCase("2.2*2", Result = "4.4")]
        public string SimplMath(string input)
        {
            return EvalProgram.Process(input);
        }

        [TestCase("a+a", "{a:2}", Result = "4")]
        [TestCase("a*2.5-somethin", "{a:2, somethin:3.4}", Result = "1.6")]
        public string Constants(string input, string json)
        {
            return EvalProgram.Process(input, JObject.Parse(json));
        }

        [TestCase("12 12", Result = "error")]
        [TestCase("100 000 + 134 405", Result = "error")]
        public string SpacesInExpression(string input)
        {
            return EvalProgram.Process(input);
        }
        
        [TestCase("0/0", Result = "error")]
        [TestCase("1.1/0", Result = "error")]
        [TestCase("1.1/(1.2-0.6*2)", Result = "error")]
        public string DivisionByZero(string input)
        {
            return EvalProgram.Process(input);
        }
    }
}
