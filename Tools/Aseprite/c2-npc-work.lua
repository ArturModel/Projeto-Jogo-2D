local source=assert(app.params.source);local root=assert(app.params.root);local pc=app.pixelColor
local function c(r,g,b) return pc.rgba(r,g,b,255) end
local ink=c(22,22,22);local skin=c(153,113,74);local highlight=c(185,144,92)
local shirts={tonao=c(117,47,39),nair=c(88,72,48),mila=c(155,119,54),celina=c(135,117,83),bento=c(33,34,32),alcina=c(57,67,57),clarice=c(166,155,120),dario=c(53,89,122)}
local function rect(im,x,y,w,h,col) for j=y,y+h-1 do for i=x,x+w-1 do if i>=0 and i<64 and j>=0 and j<64 then im:drawPixel(i,j,col) end end end end
local function line(im,x,y,xx,yy,w,col) local n=math.max(math.abs(xx-x),math.abs(yy-y),1) for k=0,n do rect(im,math.floor(x+(xx-x)*k/n+.5)-math.floor(w/2),math.floor(y+(yy-y)*k/n+.5),w,1,col) end end
for name,shirt in pairs(shirts) do
 local src=app.open(source..'/Npc_'..name..'.png');local base=src.cels[1].image;local s=Sprite(64,64,ColorMode.RGB);local sheet=Image(512,64,ColorMode.RGB)
 for f=0,7 do
  local im=Image(64,64,ColorMode.RGB);im:drawImage(base)
  if f<4 then
   -- Cotovelo e antebraço alcançam a superfície à direita do corpo; ferramenta fica presa à mão.
   rect(im,39,31,5,12,0);line(im,37,30,41,33,5,ink);line(im,37,30,41,33,3,shirt)
   local handY=36+(f%2);line(im,41,33,45,handY,4,ink);line(im,41,33,45,handY,2,shirt)
   rect(im,44,handY,3,3,skin);im:drawPixel(44,handY,highlight)
   if name=='tonao' then
    for y=handY-3,handY+3 do for x=47,53 do if (x-50)^2+(y-handY)^2<12 then im:drawPixel(x,y,c(114,72,42)) end end end
    rect(im,47,handY-1,4,2,c(149,163,140))
   elseif name=='nair' or name=='bento' then rect(im,46,handY,8,3,c(157,164,138));rect(im,46,handY,7,1,c(198,193,154))
   elseif name=='mila' then line(im,47,handY,53,handY-4,1,c(155,163,158));rect(im,46,handY,3,3,c(122,66,40))
   elseif name=='celina' then rect(im,46,handY,8,3,c(130,95,89));rect(im,48,handY,1,3,c(173,140,121))
   elseif name=='alcina' then line(im,46,handY,49+(f%2),55,2,c(111,81,45));rect(im,46,54,8,3,c(108,100,64))
   else rect(im,46,handY,7,4,c(188,181,145));rect(im,47,handY+1,5,1,c(91,89,78)) end
  else
   -- Passagem curta: mantém volume/roupa e alterna apenas o apoio inferior de dois pixels.
   local phase=f-4;local dx=({-1,0,1,0})[phase+1]
   for y=44,55 do for x=22,41 do im:drawPixel(x,y,0) end end
   for y=44,55 do for x=22,41 do local value=base:getPixel(x,y);if pc.rgbaA(value)>0 then local shift=x<32 and dx or -dx;im:drawPixel(x+shift,y-(x<32 and (phase%2) or 0),value) end end end
  end
  sheet:drawImage(im,Point(f*64,0));if f>0 then s:newEmptyFrame() end;s:newCel(s.layers[1],f+1,im,Point(0,0));s.frames[f+1].duration=.18
 end
 s:newTag(1,4).name='trabalho';s:newTag(5,8).name='passagem';s:saveAs(root..'/Trabalho_'..name..'.aseprite');if app.params.preview then s:saveCopyAs(app.params.preview..'/Trabalho_'..name..'.gif') end;s:close()
 local out=Sprite(512,64,ColorMode.RGB);out.cels[1].image=sheet;out:saveAs(root..'/Trabalho_'..name..'.png');out:close();src:close()
end
print('8 moradores: contato mão/ferramenta e apoio de caminhada em tags separadas.')
