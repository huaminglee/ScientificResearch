function Run( file, p1 , isClose)
{
  try
  {
    //alert ('Will run file=  '+file+ ' \n paras = ' +p1 +' is colose '+ isClose );
    
    var obj = new ActiveXObject("LoadModule.coTest");
    obj.LoadExe( file, p1 );
    //obj.LoadExe( 'C:\ds2002\Link.exe',p1 );
    //obj.LoadExe( 'Notepad.exe' );
    obj = null;
    if (isClose=='0')
       return ;
    else
      window.close();
  }
  catch(e)
  {
  
  var msg='';
   
    
//   msg+='������û����ȷ������IE. \t\n �밴�����²���ִ�л�����ϵͳ����Ա��������⡣';
//   msg+='\t\n 1, ��IE�˵� ����->ѡ��.';
//   msg+='\t\n 2, �ڳ����ǩ�У�Internet��ʱ�ļ��е�����->ѡ��ÿ�η��ʴ���ҳʱ��顣';
//   msg+='\t\n 3, �ڰ�ȫ��ǩ�У�������վ��->��վ��ѷ�����IP�������棬ע�ⲻҪ��https://��';
//   msg+='\t\n 4, ����˽��ǩ�У��������ֹ�������ڣ��ѶԹ�ȥ������������';
//   msg+='\t\n 5, �ڿ�ʼ������ regsvr32  LoadMod.dll ';
//   msg+='\t\n ������ ���ǲ���������װIE���������������google ��������qq ��������';
//   msg+='\t\n        �������������������Ȼ����������⣬����ѯϵͳ����Ա��';
   
   
    alert( e.message );
   // alert(msg+' ������Ϣ:\t\n'+e.message );
     return ;
  } 
} 

function RunLink( p )
{
   var obj = new ActiveXObject("LoadModule.coTest")
   obj.LoadExe( 'C:\ds2002\Link.exe', p )
   obj = null
}
