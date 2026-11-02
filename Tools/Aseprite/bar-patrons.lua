-- Poses sentadas com cadeira integrada: mesma arte em exploração e perspectiva.
local root=assert(app.params.root);local pc=app.pixelColor
local function c(r,g,b)return pc.rgba(r,g,b,255)end
local ink=c(24,25,21);local pants=c(64,51,38);local skin=c(169,124,80)
local function box(im,x,y,w,h,col)for j=y,y+h-1 do for i=x,x+w-1 do im:drawPixel(i,j,col)end end end
local function line(im,x,y,xx,yy,col)
 local n=math.max(math.abs(xx-x),math.abs(yy-y),1)
 for k=0,n do box(im,math.floor(x+(xx-x)*k/n+.5),math.floor(y+(yy-y)*k/n+.5),2,2,col)end
end
local thermal=Image(20,38,ColorMode.RGB)
box(thermal,5,7,11,27,ink);box(thermal,6,8,9,24,c(35,58,44));box(thermal,7,9,2,22,c(58,83,63));box(thermal,14,11,3,17,ink);box(thermal,15,13,1,12,c(94,111,83));box(thermal,7,3,7,5,c(38,42,35));box(thermal,8,2,5,2,c(120,127,108));box(thermal,6,32,9,2,c(112,121,100))
local td=Sprite(20,38,ColorMode.RGB);td.cels[1].image=thermal;td:saveAs(root..'/Arte/Producao/Bar2004/TermicaBar.aseprite');td:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Art/Props/TermicaBar.png');td:close()
local mate=Image(24,32,ColorMode.RGB)
-- Porongo abaulado, boca larga com montanha de erva e bomba prateada.
for y=12,26 do for x=3,19 do if ((x-11)/9)^2+((y-19)/9)^2<=1 then mate:drawPixel(x,y,c(71,43,24))end end end
for y=13,24 do for x=5,17 do if ((x-11)/7)^2+((y-18)/8)^2<=1 then mate:drawPixel(x,y,c(126,79,38))end end end
box(mate,5,10,14,4,c(175,144,83));box(mate,6,9,12,3,c(44,66,28));box(mate,7,7,8,4,c(80,106,40));box(mate,8,6,5,2,c(113,132,54));box(mate,7,11,3,2,c(133,144,61));box(mate,6,26,11,2,c(53,36,24))
line(mate,14,12,20,2,c(66,73,63));line(mate,15,11,21,2,c(183,190,166));box(mate,19,1,4,2,c(204,208,190));box(mate,6,15,2,7,c(154,103,52))
local md=Sprite(24,32,ColorMode.RGB);md.cels[1].image=mate;md:saveAs(root..'/Arte/Producao/Bar2004/CuiaBar.aseprite');md:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Art/Props/CuiaBar.png');md:close()
local names={'Npc_Frequentador','Npc_Chico','Npc_Ramiro','Npc_Motorista'}
for index,name in ipairs(names)do
 local doc=app.open(root..'/Projeto Jogo 2D/Assets/Resources/Art/Characters/'..name..'.png');local source=doc.cels[1].image
 local ch=app.open(root..'/Projeto Jogo 2D/Assets/Resources/Art/Props/CadeiraPlasticaBar.png');local chair=ch.cels[1].image
 local atlas=Image(512,64,ColorMode.RGB);local anim=Sprite(64,64,ColorMode.RGB)
 for f=0,7 do
  if f>0 then anim:newEmptyFrame()end
  local im=Image(64,64,ColorMode.RGB)
  -- Costas e assento ficam atrás do corpo; pés apoiados abaixo dos joelhos.
  for y=0,43 do for x=0,27 do im:drawPixel(18+x,17+y,chair:getPixel(x,y))end end
  local side=index%2==0;local facing=index==2 and -1 or 1
  for y=0,40 do for x=0,63 do
   local v=source:getPixel(x,y)
   if pc.rgbaA(v)>0 then
    local xx=side and math.floor(32+(x-32)*.78) or x
    im:drawPixel(xx,y+5,v)
   end
  end end
  local function limb(x,y,xx,yy)
   for d=-3,3 do line(im,x+d,y,xx+d,yy,ink)end
   for d=-2,2 do line(im,x+d,y,xx+d,yy,pants)end
   line(im,x-1,y,xx-1,yy,c(88,71,48))
  end
  -- Coxas apoiadas no assento, joelhos dobrados, canelas verticais e botas.
  if side then
   limb(30,44,39,47);limb(38,48,37,54)
   line(im,35,56,43,56,ink);line(im,36,55,41,55,c(104,88,57))
   limb(28,44,29,49);limb(28,50,27,55);line(im,23,57,30,57,ink)
  else
   limb(28,44,25,48);limb(25,49,26,54)
   limb(35,44,38,48);limb(38,49,37,54)
   line(im,22,56,28,56,ink);line(im,35,56,42,56,ink)
   line(im,23,55,27,55,c(104,88,57));line(im,36,55,40,55,c(104,88,57))
  end
  -- Apoio na mesa / pegar / erguer / beber / baixar. Mate inclui térmica e serviço.
  local sip=f==3
  local hx=sip and 35 or f==2 and 39 or 42;local hy=sip and 28 or f==2 and 35 or f==4 and 33 or f==5 and 39 or 43
  line(im,39,37,hx,hy,ink);line(im,39,36,hx,hy,skin)
  if index==3 then
   if f==6 or f==7 then
    line(im,25,38,35,34,ink);line(im,25,37,35,33,skin)
    for by=0,12 do for bx=0,5 do
     local xx=f==7 and math.floor(35+bx*.8+by*.5) or 35+bx
     local yy=f==7 and math.floor(30+by*.8-bx*.5) or 30+by
     im:drawPixel(xx,yy,(bx==0 or bx==5) and ink or c(45,73,55))
    end end
    box(im,f==7 and 42 or 34,f==7 and 39 or 29,5,3,c(119,125,110))
    line(im,35,35,43,40,c(177,190,165));line(im,42,41,42,44,c(166,184,167))
   end
   if f~=0 then
   for my=0,31 do for mx=0,23 do
    local v=mate:getPixel(mx,my)
    if pc.rgbaA(v)>0 then im:drawPixel(hx-4+math.floor(mx*.45),hy-8+math.floor(my*.45),v)end
   end end
   end
  else
   box(im,hx-2,hy-1,5,8,c(142,98,36));box(im,hx-1,hy,3,5,c(205,159,66));box(im,hx-2,hy-2,5,2,c(224,217,185))
  end
  if side and facing<0 then local mirrored=Image(64,64,ColorMode.RGB);for y=0,63 do for x=0,63 do mirrored:drawPixel(63-x,y,im:getPixel(x,y))end end;im=mirrored end
  if f==0 then anim.cels[1].image=im else anim:newCel(anim.layers[1],f+1,im,Point(0,0))end
  anim.frames[f+1].duration=.35;atlas:drawImage(im,Point(f*64,0))
 end
 local tag=anim:newTag(1,8);tag.name=index==3 and 'mate_servir_tomar' or 'cerveja_pegar_tomar'
 anim:saveAs(root..'/Arte/Producao/Bar2004/'..name..'_Rotina.aseprite');anim:saveCopyAs(root..'/Arte/Producao/Bar2004/'..name..'_Rotina.gif');anim:close()
 local sheet=Sprite(512,64,ColorMode.RGB);sheet.cels[1].image=atlas;sheet:saveAs(root..'/Projeto Jogo 2D/Assets/Resources/Art/Characters/'..name..'_Rotina.png');sheet:close();doc:close();ch:close()
end
