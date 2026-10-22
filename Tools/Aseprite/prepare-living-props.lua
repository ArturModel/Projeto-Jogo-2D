local root=assert(app.params.root);local atlas=app.open(assert(app.params.source));local src=atlas.cels[1].image
local civic=app.params.kind=='civic'
local names=civic and {'BancoPraca','QuadroAvisos','Fonte','MesaPublica','BancadaCostura','BancadaOficina','Sofa','Sepultura'} or {'Prateleira','Geladeira','Caixa','Cafe','BalcaoBar','Chimarrao','Televisor','Fogao'}
-- Gutters conferidos visualmente: a geração não produziu células com largura exatamente igual.
local cuts=civic and {{0,470,880,1315,1774},{0,458,880,1358,1774}} or {{0,470,810,1305,1774},{0,515,965,1315,1774}}
if app.params.kind=='extras' then
 names={'CadeiraMadeira','MesaMadeira','ArmarioMadeira','ArquivoMetal','CaixasManutencao','RadioAntigo','JanelaCortina','ArmarioVelas'}
 cuts={{0,440,905,1360,1774},{0,440,900,1365,1774}}
end
for i,name in ipairs(names) do
  local row=math.floor((i-1)/4);local col=(i-1)%4;local x0=math.floor(cuts[row+1][col+1]*src.width/1774);local x1=math.floor(cuts[row+1][col+2]*src.width/1774)-1
  local y0=math.floor(row*src.height/2);local y1=math.floor((row+1)*src.height/2)-1
  local l,t,r,b=x1,y1,x0,y0
  for y=y0,y1 do for x=x0,x1 do if app.pixelColor.rgbaA(src:getPixel(x,y))>128 then l=math.min(l,x);t=math.min(t,y);r=math.max(r,x);b=math.max(b,y) end end end
  local factor=math.min(112/(r-l+1),104/(b-t+1));local w,h=math.floor((r-l+1)*factor+.5),math.floor((b-t+1)*factor+.5)
  local spr=Sprite(128,128,ColorMode.RGB);local im=Image(128,128,ColorMode.RGB);local ox,oy=math.floor((128-w)/2),112-h
  for y=0,h-1 do for x=0,w-1 do im:drawPixel(ox+x,oy+y,src:getPixel(l+math.floor(x*(r-l+1)/w),t+math.floor(y*(b-t+1)/h))) end end
  spr.cels[1].image=im;spr:saveAs(root..'/'..name..'.aseprite');spr:saveCopyAs(root..'/'..name..'.png');spr:close()
end
atlas:close();print('8 props completos exportados, 128 pixels, pés a 16 pixels da base.')
