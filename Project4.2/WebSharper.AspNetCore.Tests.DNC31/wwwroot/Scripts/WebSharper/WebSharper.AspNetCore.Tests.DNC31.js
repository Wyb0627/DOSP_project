(function(Global)
{
 "use strict";
 var WebSharper,AspNetCore,Tests,WebSocketClient,Website,SomeRecord,Client,Task,SC$1,WebSharper$AspNetCore$Tests$DNC31_JsonDecoder,WebSharper$AspNetCore$Tests$DNC31_JsonEncoder,WebSharper$AspNetCore$Tests$DNC31_Templates,UI,Doc,Concurrency,WebSocket,Client$1,WithEncoding,JSON,IntelliFactory,Runtime,Remoting,AjaxRemotingProvider,Utils,Arrays,$,Var$1,Templating,Runtime$1,Server,ProviderBuilder,Handler,TemplateInstance,ListModel,List,ClientSideJson,Provider,Client$2,Templates;
 WebSharper=Global.WebSharper=Global.WebSharper||{};
 AspNetCore=WebSharper.AspNetCore=WebSharper.AspNetCore||{};
 Tests=AspNetCore.Tests=AspNetCore.Tests||{};
 WebSocketClient=Tests.WebSocketClient=Tests.WebSocketClient||{};
 Website=Tests.Website=Tests.Website||{};
 SomeRecord=Website.SomeRecord=Website.SomeRecord||{};
 Client=Website.Client=Website.Client||{};
 Task=Client.Task=Client.Task||{};
 SC$1=Global.StartupCode$WebSharper_AspNetCore_Tests_DNC31$Website=Global.StartupCode$WebSharper_AspNetCore_Tests_DNC31$Website||{};
 WebSharper$AspNetCore$Tests$DNC31_JsonDecoder=Global.WebSharper$AspNetCore$Tests$DNC31_JsonDecoder=Global.WebSharper$AspNetCore$Tests$DNC31_JsonDecoder||{};
 WebSharper$AspNetCore$Tests$DNC31_JsonEncoder=Global.WebSharper$AspNetCore$Tests$DNC31_JsonEncoder=Global.WebSharper$AspNetCore$Tests$DNC31_JsonEncoder||{};
 WebSharper$AspNetCore$Tests$DNC31_Templates=Global.WebSharper$AspNetCore$Tests$DNC31_Templates=Global.WebSharper$AspNetCore$Tests$DNC31_Templates||{};
 UI=WebSharper&&WebSharper.UI;
 Doc=UI&&UI.Doc;
 Concurrency=WebSharper&&WebSharper.Concurrency;
 WebSocket=AspNetCore&&AspNetCore.WebSocket;
 Client$1=WebSocket&&WebSocket.Client;
 WithEncoding=Client$1&&Client$1.WithEncoding;
 JSON=Global.JSON;
 IntelliFactory=Global.IntelliFactory;
 Runtime=IntelliFactory&&IntelliFactory.Runtime;
 Remoting=WebSharper&&WebSharper.Remoting;
 AjaxRemotingProvider=Remoting&&Remoting.AjaxRemotingProvider;
 Utils=WebSharper&&WebSharper.Utils;
 Arrays=WebSharper&&WebSharper.Arrays;
 $=Global.jQuery;
 Var$1=UI&&UI.Var$1;
 Templating=UI&&UI.Templating;
 Runtime$1=Templating&&Templating.Runtime;
 Server=Runtime$1&&Runtime$1.Server;
 ProviderBuilder=Server&&Server.ProviderBuilder;
 Handler=Server&&Server.Handler;
 TemplateInstance=Server&&Server.TemplateInstance;
 ListModel=UI&&UI.ListModel;
 List=WebSharper&&WebSharper.List;
 ClientSideJson=WebSharper&&WebSharper.ClientSideJson;
 Provider=ClientSideJson&&ClientSideJson.Provider;
 Client$2=UI&&UI.Client;
 Templates=Client$2&&Client$2.Templates;
 WebSocketClient.WebSocketTest=function(str,endpoint)
 {
  var container,b;
  function writen(fmt)
  {
   return fmt(function(s)
   {
    var x;
    x=self.document.createTextNode(s+"\n");
    container.elt.appendChild(x);
   });
  }
  container=Doc.Element("pre",[],[]);
  Concurrency.Start((b=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind(WithEncoding.ConnectStateful(function(a)
   {
    return JSON.stringify((WebSharper$AspNetCore$Tests$DNC31_JsonEncoder.j())(a));
   },function(a)
   {
    return(WebSharper$AspNetCore$Tests$DNC31_JsonDecoder.j())(JSON.parse(a));
   },endpoint,function()
   {
    var b$1;
    b$1=null;
    return Concurrency.Delay(function()
    {
     return Concurrency.Return([0,function(state)
     {
      return function(msg)
      {
       var b$2;
       b$2=null;
       return Concurrency.Delay(function()
       {
        var data,x,b$3;
        return msg.$==3?Concurrency.Return(state):msg.$==2?Concurrency.Return(state):msg.$==1?(writen(function($1)
        {
         return $1("WebSocket connection error!");
        }),Concurrency.Return(state)):(data=msg.$0,Concurrency.Combine(data.$==0?(((writen(Runtime.Curried3(function($1,$2,$3)
        {
         return $1("Response2 "+Global.String($2)+" (state: "+Global.String($3)+")");
        })))(data.$0))(state),Concurrency.Zero()):(x=data.$0,(Concurrency.Start((b$3=null,Concurrency.Delay(function()
        {
         (new AjaxRemotingProvider.New()).Send("WebSharper.AspNetCore.Tests.DNC31:WebSharper.AspNetCore.Tests.WebSocketClient.printit:-1410224646",[x]);
         return x.length>1?((writen(function($1)
         {
          return function($2)
          {
           return $1(Utils.toSafe($2));
          };
         }))(x),Concurrency.Zero()):Concurrency.Zero();
        })),null),Concurrency.Zero())),Concurrency.Delay(function()
        {
         return Concurrency.Return(state+1);
        })));
       });
      };
     }]);
    });
   }),function(a)
   {
    var m,r;
    Arrays.length((new AjaxRemotingProvider.New()).Sync("WebSharper.AspNetCore.Tests.DNC31:WebSharper.AspNetCore.Tests.WebSocketClient.spiltWithNoAsync:-1362869456",[str]))>2?(m=Arrays.get((new AjaxRemotingProvider.New()).Sync("WebSharper.AspNetCore.Tests.DNC31:WebSharper.AspNetCore.Tests.WebSocketClient.spiltWithNoAsync:-1362869456",[str]),2),m==="Register"?a.Post({
     $:4,
     $0:str
    }):m==="Subscribe"?a.Post({
     $:6,
     $0:str
    }):m==="Tweet"?a.Post({
     $:5,
     $0:str
    }):m==="ReTweet"?a.Post({
     $:12,
     $0:str
    }):m==="Query"?a.Post({
     $:7,
     $0:str
    }):m==="QueryTag"?a.Post({
     $:8,
     $0:str
    }):m==="QueryAt"?a.Post({
     $:9,
     $0:str
    }):m==="Login"?a.Post({
     $:10,
     $0:str
    }):m==="Logout"?a.Post({
     $:11,
     $0:str
    }):(new AjaxRemotingProvider.New()).Send("WebSharper.AspNetCore.Tests.DNC31:WebSharper.AspNetCore.Tests.WebSocketClient.printit:-1410224646",["Error"])):a.Post({
     $:2,
     $0:str
    });
    $.ajax((r={},r.url="/ws.txt",r.method="GET",r.success=function(x)
    {
     return(writen(function($1)
     {
      return function($2)
      {
       return $1(Utils.toSafe($2));
      };
     }))(x);
    },r.error=function(a$1,a$2,e)
    {
     return(writen(function($1)
     {
      return function($2)
      {
       return $1("KO: "+Utils.toSafe($2)+".");
      };
     }))(e);
    },r));
    return Concurrency.Zero();
   });
  })),null);
  return container;
 };
 SomeRecord.New=function(Name)
 {
  return{
   Name:Name
  };
 };
 Task.New=function(Name,Done)
 {
  return{
   Name:Name,
   Done:Done
  };
 };
 Client.Main$127$20=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Logout",wsep);
  };
 };
 Client.Main$125$22=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Login",wsep);
  };
 };
 Client.Main$123$21=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",QueryAt",wsep);
  };
 };
 Client.Main$121$22=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",QueryTag",wsep);
  };
 };
 Client.Main$119$19=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+", ,Query",wsep);
  };
 };
 Client.Main$117$23=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Subscribe",wsep);
  };
 };
 Client.Main$115$21=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",ReTweet",wsep);
  };
 };
 Client.Main$112$19=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Tweet",wsep);
  };
 };
 Client.Main$110$22=function(wsep)
 {
  return function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Register",wsep);
  };
 };
 Client.Main=function(aboutPageLink,wsep)
 {
  var b,_this,W,name,_this$1,R,b$1,_this$2,t,t$1,t$2,t$3,t$4,t$5,t$6,t$7,t$8,_this$3,p,i;
  return(b=(_this=(W=(name=(new AjaxRemotingProvider.New()).Sync("WebSharper.AspNetCore.Tests.DNC31:WebSharper.AspNetCore.Tests.Website.rpcToString:-242087931",[]),(Var$1.Set(Client.username(),name),WebSocketClient.WebSocketTest(Client.username().Get(),wsep))),(_this$1=(R=Doc.Async((b$1=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("WebSharper.AspNetCore.Tests.DNC31:WebSharper.AspNetCore.Tests.Website.DoSomething:32829901",[]),function(a)
   {
    return Concurrency.Return(Doc.Element("div",[],[Doc.TextNode(a.Name)]));
   });
  }))),(_this$2=(t=(t$1=(t$2=(t$3=(t$4=(t$5=(t$6=(t$7=(t$8=(_this$3=new ProviderBuilder.New$1(),(_this$3.h.push({
   $:8,
   $0:"login",
   $1:Client.Login()
  }),_this$3)),(t$8.h.push(Handler.EventQ2(t$8.k,"register",function()
  {
   return t$8.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Register",wsep);
  })),t$8)),(t$7.h.push(Handler.EventQ2(t$7.k,"tweet",function()
  {
   return t$7.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Tweet",wsep);
  })),t$7)),(t$6.h.push(Handler.EventQ2(t$6.k,"retweet",function()
  {
   return t$6.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",ReTweet",wsep);
  })),t$6)),(t$5.h.push(Handler.EventQ2(t$5.k,"subscribe",function()
  {
   return t$5.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Subscribe",wsep);
  })),t$5)),(t$4.h.push(Handler.EventQ2(t$4.k,"query",function()
  {
   return t$4.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+", ,Query",wsep);
  })),t$4)),(t$3.h.push(Handler.EventQ2(t$3.k,"querytag",function()
  {
   return t$3.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",QueryTag",wsep);
  })),t$3)),(t$2.h.push(Handler.EventQ2(t$2.k,"queryat",function()
  {
   return t$2.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",QueryAt",wsep);
  })),t$2)),(t$1.h.push(Handler.EventQ2(t$1.k,"getlogin",function()
  {
   return t$1.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Login",wsep);
  })),t$1)),(t.h.push(Handler.EventQ2(t.k,"logout",function()
  {
   return t.i;
  },function()
  {
   WebSocketClient.WebSocketTest(Client.username().Get()+","+Client.Login().Get()+",Logout",wsep);
  })),t)),(_this$2.h.push({
   $:0,
   $0:"remotingtest",
   $1:R
  }),_this$2))),(_this$1.h.push({
   $:0,
   $0:"websockettest",
   $1:W
  }),_this$1))),(_this.h.push({
   $:1,
   $0:"aboutpagelink",
   $1:aboutPageLink
  }),_this)),(p=Handler.CompleteHoles(b.k,b.h,[["login",0]]),(i=new TemplateInstance.New(p[1],WebSharper$AspNetCore$Tests$DNC31_Templates.body(p[0])),b.i=i,i))).get_Doc();
 };
 Client.username=function()
 {
  SC$1.$cctor();
  return SC$1.username;
 };
 Client.Login=function()
 {
  SC$1.$cctor();
  return SC$1.Login;
 };
 Client.NewTaskName=function()
 {
  SC$1.$cctor();
  return SC$1.NewTaskName;
 };
 Client.Tasks=function()
 {
  SC$1.$cctor();
  return SC$1.Tasks;
 };
 SC$1.$cctor=function()
 {
  SC$1.$cctor=Global.ignore;
  SC$1.Tasks=ListModel.Create(function(task)
  {
   return task.Name;
  },List.ofArray([Task.New("Have breakfast",Var$1.Create$1(true)),Task.New("Have lunch",Var$1.Create$1(false))]));
  SC$1.NewTaskName=Var$1.Create$1("");
  SC$1.Login=Var$1.Create$1("");
  SC$1.username=Var$1.Create$1("");
 };
 WebSharper$AspNetCore$Tests$DNC31_JsonDecoder.j=function()
 {
  return WebSharper$AspNetCore$Tests$DNC31_JsonDecoder._v?WebSharper$AspNetCore$Tests$DNC31_JsonDecoder._v:WebSharper$AspNetCore$Tests$DNC31_JsonDecoder._v=(Provider.DecodeUnion(void 0,"type",[["int",[["$0","value",Provider.Id(),0]]],["string",[["$0","value",Provider.Id(),0]]]]))();
 };
 WebSharper$AspNetCore$Tests$DNC31_JsonEncoder.j=function()
 {
  return WebSharper$AspNetCore$Tests$DNC31_JsonEncoder._v?WebSharper$AspNetCore$Tests$DNC31_JsonEncoder._v:WebSharper$AspNetCore$Tests$DNC31_JsonEncoder._v=(Provider.EncodeUnion(void 0,{
   ret:12,
   logoutq:11,
   loginq:10,
   atq:9,
   tagq:8,
   subq:7,
   sub:6,
   twe:5,
   reg:4,
   mes3:3,
   mes4:2,
   "int":1,
   str:0
  },[["Request1",[["$0","str",Provider.EncodeArray(Provider.Id()),0]]],["Request2",[["$0","int",Provider.EncodeArray(Provider.Id()),0]]],["Request4",[["$0","mes4",Provider.Id(),0]]],["Request3",[["$0","mes3",Provider.Id(),0]]],["Register",[["$0","reg",Provider.Id(),0]]],["Tweet",[["$0","twe",Provider.Id(),0]]],["Subscribe",[["$0","sub",Provider.Id(),0]]],["SubQuery",[["$0","subq",Provider.Id(),0]]],["TagQuery",[["$0","tagq",Provider.Id(),0]]],["AtQuery",[["$0","atq",Provider.Id(),0]]],["LoginQuery",[["$0","loginq",Provider.Id(),0]]],["LogoutQuery",[["$0","logoutq",Provider.Id(),0]]],["ReTweet",[["$0","ret",Provider.Id(),0]]]]))();
 };
 WebSharper$AspNetCore$Tests$DNC31_Templates.body=function(h)
 {
  Templates.LoadLocalTemplates("main");
  return h?Templates.NamedTemplate("main",{
   $:1,
   $0:"body"
  },h):void 0;
 };
}(self));
