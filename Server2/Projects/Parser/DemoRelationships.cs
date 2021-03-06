﻿/////////////////////////////////////////////////////////////////////
// DemoRelationships.cs - demonstrate all four class relationships //
// Ver 1.1, 9/6/2014                                               //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2014 //
/////////////////////////////////////////////////////////////////////
/*
 * Note: 
 *   No Manual Page or Maintenance Page because this is just
 *   demonstration code.    
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeRelationships
{
    //----< reference type can only be aggregated >--------------------
    enum d { a, b, c }
    public interface C
    {
        void p();

    }
    public interface Q
    {
        void q();

    }



    public class ccllsad : C, Q
    {
        public void q()
        {

        }
        public void p()
        {

        }

    }
    public class AggregatedType  // all classes are reference types
    {
        CommandLineParser c = new CommandLineParser();
        public delegate void invoker();
        public virtual void say1()
        { }
        public virtual void say()
        {
            int[] j = { 1, 2, 3 };


            for (int i = 0; i < 10; i++)
                foreach (int k in j)
                    Console.Write("\n  hello - {0}", atStr);
        }
        protected string atStr = "my type is AggregatedType";
    }
    //----< Inheritance >----------------------------------------------

    

    public class DerivedType : Analyzer
    {
        public invoker inv1;

        public DerivedType()
        {

            atStr = "my type is DerivedType";
        }
        public DerivedType property
        {
            get;
            set;
        }



        public override void say()
        {
            inv1.Invoke();
            Console.Write("\n  hello from a derived type - {0}", atStr);
        }
    }
    //----< value type will be compos/ed >------------------------------



    struct ComposedType  // structs are value types
    {
        public void say()
        {
            Console.Write("\n  hello - my type is ComposedType");
            Console.Write("\n  my string is \"{0}\"", s);
            Console.Write("\n  my double is {0}", d);
            Console.Write("\n  my int is    {0}", i);
        }
        public string s;
        public double d;
        public int i;
    }
    //----< reference type will be used >------------------------------

    
    class UsedType
    {
        public UsedType(string s)
        {
            str = s;
        }
        public void say()
        {
            Console.Write("\n  hello, I'm a used type \"{0}\"", str);
        }
        private string str;
    }

    class DemoRelationships
    {
        ComposedType ct;  // note: no new statement
        AggregatedType at = new AggregatedType();
        DerivedType dt = new DerivedType();

//        ----< constructor initializes composed data >------------------

        public DemoRelationships()
        {
            ct.s = "a string";
            ct.d = 3.1415927;
            ct.i = -3;
        }
        /*----< this function is here because static Main >------------
          ----< can't directly access nonstatic data ct   >------------*/

        public void say(UsedType ut)
        {
            ct.say();
            ut.say();
        }
 //       ----< entry point >--------------------------------------------



        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrating Type Relationships");
            Console.Write("\n ==================================\n");

            DemoRelationships p = new DemoRelationships();
            p.at.say();
            p.dt.say();

            ComposedType ct = new ComposedType();
            ct.d = 1;

            UsedType ut = new UsedType("holding this string");
            p.say(ut);
            Console.Write("\n\n");
        }
    }
    public interface A : C
    {

    }
}
