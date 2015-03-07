///////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Parser rules specific to an application      //
// ver 2.2                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Inspiron 17R 5721, Win 8.1                      //
//              Microsoft Visual Studio 2013 Ultimate                //
// Application: Code Analyzer for CSE681, Project #2, Fall 2014      //
// Author:      Akhil Panchal, MS Computer Science,
//              Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 2.3 : 08 Oct 2014
 * - Added Rules for Delegate and Enum Type Detection
 * - Added Rules for detecting type relationships:
 *      1. Inheritence
 *      2. Composition
 *      3. Aggregation and
 *      4. Using
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications (not needed for Project #2):
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 *   
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeAnalysis
{
  public class Elem  // holds scope information
  {
    public string type { get; set; }
    public string name { get; set; }
    public int begin { get; set; }
    public int end { get; set; }
    public int complexity { get; set; }
    public string filename { get; set; }
    public override string ToString()
    {
      StringBuilder temp = new StringBuilder();
      temp.Append("{");
      temp.Append(String.Format("{0,-10}", type)).Append(" : ");
      temp.Append(String.Format("{0,-10}", name)).Append(" : ");
      temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
      temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
      temp.Append("}");
      return temp.ToString();
    }
  }
  public class Type  // holds detected Type information
  {
      public string type { get; set; }
      public string name { get; set; }
      public string nspace { get; set; }
      public string filename { get; set; }
  }
  public class Relationship  // holds detected Type Relationships
  {
      public string type1 { get; set; }
      public string filename1 { get; set; }
      public string type2 { get; set; }
      public string filename2 { get; set; }
      public string name1 { get; set; }
      public string name2 { get; set; }
      public string relationship { get; set; }

  }

  public class Repository
  {
    ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
    List<Elem> locations_ = new List<Elem>();
    List<Type> typeTable_ = new List<Type>();
    List<Relationship> relationshipTable_ = new List<Relationship>();
    List<string> stack2_ = new List<string>();
    List<string> stack_value_ = new List<string>();
    static Repository instance;
    private int complexity=0;
    private string curr_type;
    public string getcurr_type()
    {
        return curr_type;
    }
    public void setcurr_type(string t)
    {
        curr_type = t;
    }
    public int getcomplex()
    { 
        return complexity;
    }
    public void setcomplexity(int complex)
    {
        complexity=complex;
    }
    public void increamentcomplex()
    {
        complexity++;
    }
    public Repository()
    {
      instance = this;
    }

    public static Repository getInstance()
    {
      return instance;
    }

    // provides all actions access to current semiExp
    public CSsemi.CSemiExp semi
    {
      get;
      set;
    }

    // semi gets line count from toker who counts lines
    // while reading from its source

    public int lineCount  // saved by newline rule's action
    {
      get { return semi.lineCount; }
    }
    public int prevLineCount  // not used in this demo
    {
      get;
      set;
    }
    // enables recursively tracking entry and exit from scopes

    public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
    {
      get { return stack_; } 
    }

    // enables recursively tracking entry and exit from scopes
    public List<string> stack2  // pushed and popped by scope rule's action
    {
        get { return stack2_; }
    }

    public List<string> stack_value // pushed and popped by scope rule's action
    {
        get { return stack_value_; }
    }
  
    // the locations table is the result returned by parser's actions
    // in this demo

    public List<Elem> locations
    {
      get { return locations_; }
    }

    public List<Type> typetable
    {
        get { return typeTable_; }
    }
    public List<Relationship> relationshiptable
    {
        get { return relationshipTable_; }
    }
  }

  public class Push2 : AAction
  {
      Repository repo_;
      public Push2(Repository repo)
      {
          repo_ = repo;
      }
      //Action after detection of relationships
      public override void doAction(CSsemi.CSemiExp semi)
      {
          Elem elem = new Elem();
          Relationship r1 = new Relationship();
          if (semi[0] == "class" || semi[0] == "interface" || semi[0] == "struct")
          {
              repo_.stack2.Add(semi[0]);
              repo_.stack2.Add(semi[1]);
              return;
          }
          if(semi[0] == "inherits")
          {
              r1.type1 = semi[1];
              r1.name1 = semi[2];
              r1.relationship = semi[0];
              r1.type2 = semi[3];
              r1.name2 = semi[4];
              repo_.relationshiptable.Add(r1);
              return;
              
          }
          if (semi[0] == "aggregates" || semi[0] == "uses" ||semi[0] == "composes")
          {
              int cnt=repo_.stack2.Count;
              r1.type1 = repo_.stack2[cnt - 2];
              r1.name1 = repo_.stack2[cnt - 1];
              r1.relationship = semi[0];
              r1.type2 = semi[1];
              r1.name2 = semi[2];
              repo_.relationshiptable.Add(r1);
              return;
          }          
      }
  }

  /////////////////////////////////////////////////////////
  // pushes scope info on stack when entering new scope

    public class PushStack : AAction
  {
    Repository repo_;    
    public PushStack(Repository repo)
    {
      repo_ = repo;
    }
    public override void doAction(CSsemi.CSemiExp semi)
    {
      Elem elem = new Elem();
      elem.type = semi[0];  // expects type
      elem.name = semi[1];  // expects name
      elem.begin = repo_.semi.lineCount - 1;
      elem.end = 0;     
      if(elem.type == "function")
      {
          repo_.setcomplexity(0);
      }
      if (elem.type == "bracelesscontrol")
      {
          repo_.increamentcomplex();
          return;
      }
      repo_.stack.push(elem);
      repo_.increamentcomplex();
      repo_.locations.Add(elem);
      if (elem.type == "class" || elem.type == "interface" || elem.type == "struct" || elem.type == "enum" || elem.type == "delegate")
      {
          Type t1 = new Type();
          t1.type = elem.type;
          t1.name = elem.name;
          foreach (Elem e in repo_.locations)
          {
              if (e.type == "namespace")
                  t1.nspace = e.name;
          }
          repo_.typetable.Add(t1);
      }
      if (AAction.displaySemi)
      {
        Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
        Console.Write("entering ");
        string indent = new string(' ', 2 * repo_.stack.count);
        Console.Write("{0}", indent);
        this.display(semi); // defined in abstract action
      }
      if(AAction.displayStack)
        repo_.stack.display();
    }
  }

     /////////////////////////////////////////////////////////
  // pops scope info from stack when leaving scope

  public class PopStack : AAction
  {
    Repository repo_;

    public PopStack(Repository repo)
    {
      repo_ = repo;
    }
    public override void doAction(CSsemi.CSemiExp semi)
    {
      Elem elem;
      try
      {
        elem = repo_.stack.pop();
        for (int i = 0; i < repo_.locations.Count; ++i )
        {
          Elem temp = repo_.locations[i];
          if (elem.type == temp.type)
          {
            if (elem.name == temp.name)
            {
              if ((repo_.locations[i]).end == 0)
              {
                (repo_.locations[i]).end = repo_.semi.lineCount;
                if (elem.type == "function")
                {
                    (repo_.locations[i]).complexity = repo_.getcomplex();
                    repo_.setcomplexity(0);
                }
                break;
              }
            }
          }
        }
      }
      catch
      {
        Console.Write("popped empty stack on semiExp: ");
        semi.display();
        return;
      }
      CSsemi.CSemiExp local = new CSsemi.CSemiExp();
      local.Add(elem.type).Add(elem.name);
      if(local[0] == "control")
        return;
      if (AAction.displaySemi)
      {
        Console.Write("\n  line# {0,-5}", repo_.semi.lineCount);
        Console.Write("leaving  ");
        string indent = new string(' ', 2 * (repo_.stack.count + 1));
        Console.Write("{0}", indent);
        this.display(local); // defined in abstract action
      }
    }
  }
  ///////////////////////////////////////////////////////////
  // action to print function signatures - not used in demo

  public class PrintFunction : AAction
  {
    Repository repo_;

    public PrintFunction(Repository repo)
    {
      repo_ = repo;
    }
    public override void display(CSsemi.CSemiExp semi)
    {
      Console.Write("\n    line# {0}", repo_.semi.lineCount - 1);
      Console.Write("\n    ");
      for (int i = 0; i < semi.count; ++i)
        if (semi[i] != "\n" && !semi.isComment(semi[i]))
          Console.Write("{0} ", semi[i]);
    }
    public override void doAction(CSsemi.CSemiExp semi)
    {
      this.display(semi);
    }
  }
  /////////////////////////////////////////////////////////
  // concrete printing action, useful for debugging

  public class Print : AAction
  {
    Repository repo_;

    public Print(Repository repo)
    {
      repo_ = repo;
    }
    public override void doAction(CSsemi.CSemiExp semi)
    {
      Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
      this.display(semi);
    }
  }
  /////////////////////////////////////////////////////////
  // rule to detect namespace declarations

  public class DetectNamespace : ARule
  {
    public override bool test(CSsemi.CSemiExp semi)
    {
      int index = semi.Contains("namespace");
      if (index != -1)
      {
        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
        // create local semiExp with tokens for type and name
        local.displayNewLines = false;
        local.Add(semi[index]).Add(semi[index + 1]);
        doActions(local);
        return true;
      }
      return false;
    }
  }
  /////////////////////////////////////////////////////////
  // rule to dectect class definitions

  public class DetectClass : ARule
  {
    public override bool test(CSsemi.CSemiExp semi)
    {
      Repository r = Repository.getInstance();
      int indexCL = semi.Contains("class");
      int indexIF = semi.Contains("interface");
      int indexST = semi.Contains("struct");

      int index = Math.Max(indexCL, indexIF);
      index = Math.Max(index, indexST);
      if (index != -1)
      {
        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
        // local semiExp with tokens for type and name
        local.displayNewLines = false;
        local.Add(semi[index]).Add(semi[index + 1]);
        if(semi[index]=="struct")
        {
            r.stack_value.Add(semi[index]);
            r.stack_value.Add(semi[index + 1]);
        }
                
        doActions(local);
        return true;
      }
      return false;
    }
  }

  /////////////////////////////////////////////////////////
  // rule to dectect class definitions
  public class DetectDelegate : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int indexDG = semi.Contains("delegate");

          if (indexDG != -1)
          {
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              // local semiExp with tokens for type and name
              local.displayNewLines = false;
              local.Add(semi[indexDG]).Add(semi[indexDG + 2]);
              doActions(local);
              return true;
          }
          return false;
      }
  }

  /////////////////////////////////////////////////////////
  // rule to dectect Enum definitions
  public class DetectEnum : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          Repository r = Repository.getInstance();
          int indexEN = semi.Contains("enum");

          if (indexEN != -1)
          {
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              // local semiExp with tokens for type and name
              local.displayNewLines = false;
              local.Add(semi[indexEN]).Add(semi[indexEN + 1]);
              r.stack_value.Add(semi[indexEN]);
              r.stack_value.Add(semi[indexEN + 1]);              
              doActions(local);
              return true;
          }
          return false;
      }
  }

  
  /////////////////////////////////////////////////////////
  // rule to dectect function definitions
  public class DetectFunction : ARule
  {
    public static bool isSpecialToken(string token)
    {
      string[] SpecialToken = {"if","else","for","foreach","while","catch","try"};
      foreach (string stoken in SpecialToken)
        if (stoken == token)
          return true;
      return false;
    }
    public override bool test(CSsemi.CSemiExp semi)
    {
      if (semi[semi.count - 1] != "{")
        return false;

      int index = semi.FindFirst("(");
      if (index > 0 && !isSpecialToken(semi[index - 1]))
      {
        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
        local.Add("function").Add(semi[index - 1]);
        doActions(local);
        return true;
      }
      return false;
    }
  }
  /////////////////////////////////////////////////////////
  // detect entering anonymous scope
  // - expects namespace, class, and function scopes
  //   already handled, so put this rule after those
  public class DetectAnonymousScope : ARule
  {
    public override bool test(CSsemi.CSemiExp semi)
    {
      int index = semi.Contains("{");
      if (index != -1)
      {
        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
        // create local semiExp with tokens for type and name
        local.displayNewLines = false;
        local.Add("control").Add("anonymous");
        doActions(local);
        return true;
      }
      return false;
    }
  }

  /////////////////////////////////////////////////////////
  // rule to dectect Braceless Scopes
  public class DetectBracelessScope : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int i = 0, index = 0;
          for (i = 0; i < semi.count; i++)
          {
              if (DetectFunction.isSpecialToken(semi[i]))
              {
                  CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                  local.Add("bracelesscontrol").Add(semi[index]);
                  doActions(local);
                  return true;
              }
          }
          return false;
      }
  }

  /////////////////////////////////////////////////////////
  // detect leaving scope

  public class DetectLeavingScope : ARule
  {
    public override bool test(CSsemi.CSemiExp semi)
    {
      int index = semi.Contains("}");
      if (index != -1)
      {
        doActions(semi);
        return true;
      }
      return false;
    }
  }

  /////////////////////////////////////////////////////////
  // rule to dectect inheritence relationship

  public class DetectInheritence : ARule
  {      
      public static string isDefinedType(string token)
      {
          Repository rep = Repository.getInstance();
          List<Type> typetable = rep.typetable;

          foreach (Type t in typetable)
              if (t.name == token)
                  return t.type;
          return "NOTINTABLE";
      }
      public override bool test(CSsemi.CSemiExp semi)
      {
          // local semiExp with tokens for typename and relationship
          int i=0;

          int indexIH = semi.Contains(":");
          if (indexIH != -1)
          {
              for (i = indexIH+1; i < semi.count;i=i+2 )
              {
                  if (isDefinedType(semi[indexIH - 1]) != "NOTINTABLE" && isDefinedType(semi[i]) != "NOTINTABLE")
                  {
                      CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                      local.displayNewLines = false;          
                      local.Add("inherits");
                      local.Add(isDefinedType(semi[indexIH - 1]));
                      local.Add(semi[indexIH - 1]);
                      local.Add(isDefinedType(semi[i]));
                      local.Add(semi[i]);
                      doActions(local);
                  }
              }
              return false;                         
          }
          return false;
      }
  }

  /////////////////////////////////////////////////////////
  // rule to dectect aggregation relationship
  public class DetectAggregation : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          CSsemi.CSemiExp local = new CSsemi.CSemiExp();
          local.displayNewLines = false;
          int indexAG = semi.Contains("new");
          if (indexAG != -1)
          {
              if (DetectInheritence.isDefinedType(semi[indexAG + 1]) != "NOTINTABLE")
              {
                  local.Add("aggregates");
                  local.Add(DetectInheritence.isDefinedType(semi[indexAG + 1]));
                  local.Add(semi[indexAG + 1]);                  
                  doActions(local);
                  return true;
              }
          }
          return false;
      }
  }

    //Detect Type definition for pass 2
  public class DetectClassChange : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          int indexCL = semi.Contains("class");
          int indexIF = semi.Contains("interface");
          int indexST = semi.Contains("struct");
          int index = Math.Max(indexCL, indexIF);
          index = Math.Max(index, indexST);
          if (index != -1)
          { 
              CSsemi.CSemiExp local = new CSsemi.CSemiExp();
              local.displayNewLines = false;
              local.Add(semi[index]);
              local.Add(semi[index + 1]);
              doActions(local);
              return true;
          }
          return false;
      }
  }

  /////////////////////////////////////////////////////////
  // rule to dectect using relationship
  public class DetectUsing : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          CSsemi.CSemiExp local = new CSsemi.CSemiExp();
          Repository r= Repository.getInstance();
          int flag = 0; string type=null;
          if (semi[semi.count - 1] != "{")
              return false;
          int index = semi.FindFirst("(");
          if (index > 0 && !DetectFunction.isSpecialToken(semi[index - 1]))
          {
              //check in semi[index+1] is a type
              foreach(Type t in r.typetable)
                  if(t.name==semi[index+1])
                  {
                      flag = 1;
                      type = t.type;
                  }                      
              if(flag==1)
              {
                  local.Add("uses").Add(type).Add(semi[index + 1]);
                  doActions(local);
                  return true;
              }              
          }
          return false;
      }
  }

  /////////////////////////////////////////////////////////
  // rule to dectect composition relationship
  public class DetectComposition : ARule
  {
      public override bool test(CSsemi.CSemiExp semi)
      {
          CSsemi.CSemiExp local = new CSsemi.CSemiExp();
          Repository r = Repository.getInstance();
          int i=0, j=0;
          for (i = 0; i < semi.count; i++)
              if (semi[i] != "\n" && semi[i] != " ")
                  break;
          for (j = 1; j < r.stack_value.Count;j=j+2 )
          {
              if(semi[i]==r.stack_value[j])
              {
 //                 Console.WriteLine("\nstruct detected\n");
                  local.Add("composes");
                  local.Add(r.stack_value[j - 1]);
                  local.Add(r.stack_value[j]);
                  doActions(local);
                  return true;
              }
          }
          return false;
      }
  }
  public class BuildCodeAnalyzer
  {
      Repository repo = new Repository();
      public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
      {
          repo.semi = semi;
      }

      public virtual Parser buildpass1()
      {
          Parser parser = new Parser();
          // decide what to show
          AAction.displaySemi = false;
          AAction.displayStack = false;  // this is default so redundant
          // action used for namespaces, classes, and functions
          PushStack push = new PushStack(repo);
          // capture namespace info
          DetectNamespace detectNS = new DetectNamespace();
          detectNS.add(push);
          parser.add(detectNS);
          // capture class info
          DetectClass detectCl = new DetectClass();
          detectCl.add(push);
          parser.add(detectCl);
          // capture delegate info
          DetectDelegate detectDg = new DetectDelegate();
          detectDg.add(push);
          parser.add(detectDg);
          // capture enum info
          DetectEnum detectEn = new DetectEnum();
          detectEn.add(push);
          parser.add(detectEn);
          // capture function info
          DetectFunction detectFN = new DetectFunction();
          detectFN.add(push);
          parser.add(detectFN);
          // handle entering anonymous scopes, e.g., if, while, etc.
          DetectAnonymousScope anon = new DetectAnonymousScope();
          anon.add(push);
          parser.add(anon);
          //capture braceless scopes
          DetectBracelessScope detectBS = new DetectBracelessScope();
          detectBS.add(push);
          parser.add(detectBS);
          // handle leaving scopes
          DetectLeavingScope leave = new DetectLeavingScope();
          PopStack pop = new PopStack(repo);
          leave.add(pop);
          parser.add(leave);
          // parser configured
          return parser;
      }
  }
        //----------------------------------------------------------------------------------- 
    
    public class BuildCodeAnalyzer2
    {
         Repository repo = Repository.getInstance();
         public BuildCodeAnalyzer2(CSsemi.CSemiExp semi)
         {
            repo.semi = semi;
         }

        //Parser build for Pass 2
        public virtual Parser buildpass2()
        {
            Parser parser2 = new Parser();
            // action used for namespaces, classes, and functions
            Push2 push = new Push2(repo);
            // capture inheritence relationship
            DetectInheritence detectIn = new DetectInheritence();
            detectIn.add(push);
            parser2.add(detectIn);
            //capture change of class
            DetectClassChange detectCc = new DetectClassChange();
            detectCc.add(push);
            parser2.add(detectCc);           
            //capture aggregation relationship
            DetectAggregation detectAg = new DetectAggregation();
            detectAg.add(push);
            parser2.add(detectAg);
            //capture using relationship
            DetectUsing detectUs = new DetectUsing();
            detectUs.add(push);
            parser2.add(detectUs);
            //capture composition relationship
            DetectComposition detectCo = new DetectComposition();
            detectCo.add(push);
            parser2.add(detectCo);                        
            // parser configured
            return parser2;
        }
    }
}