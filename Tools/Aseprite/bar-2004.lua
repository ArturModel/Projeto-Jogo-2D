-- Original Santa Rita props; editable sources and deterministic exports.
local root=assert(app.params.root)
local pc=app.pixelColor
local function c(r,g,b,a)return pc.rgba(r,g,b,a or 255)end
local function box(im,x,y,w,h,col)for yy=y,y+h-1 do for xx=x,x+w-1 do im:drawPixel(xx,yy,col)end end end
local function ellipse(im,cx,cy,rx,ry,col)for y=math.floor(cy-ry),math.ceil(cy+ry) do for x=math.floor(cx-rx),math.ceil(cx+rx) do if ((x-cx)/rx)^2+((y-cy)/ry)^2<=1 then im:drawPixel(x,y,col)end end end end
local function save(im,name)
 local s=Sprite(im.width,im.height,ColorMode.RGB);s.layers[1].name=name..' / Santa Rita 2004';s.cels[1].image=im
 s:saveAs(root..'/Arte/Producao/Bar2004/'..name..'.aseprite');s:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Art/Props/'..name..'.png');s:close()
end
local im=Image(48,48,ColorMode.RGB)
ellipse(im,24,42,20,4,c(12,14,12,90))
box(im,9,20,4,21,c(115,115,100));box(im,35,20,4,21,c(133,134,118));box(im,12,38,4,4,c(76,81,69));box(im,32,38,4,4,c(76,81,69))
ellipse(im,24,18,22,11,c(83,89,76));ellipse(im,24,16,22,11,c(190,192,170));ellipse(im,24,14,21,10,c(220,219,197));ellipse(im,24,14,19,8,c(205,207,184))
for y=10,18,3 do for x=10,38,4 do if ((x-24)/17)^2+((y-14)/7)^2<1 then box(im,x,y,2,1,c(184,188,166))end end end
box(im,32,12,4,1,c(232,229,207));save(im,'MesaPlasticaBar')
im=Image(28,44,ColorMode.RGB);ellipse(im,14,40,12,3,c(12,14,12,70))
box(im,4,7,3,32,c(159,163,143));box(im,21,7,3,32,c(174,178,154));box(im,4,5,20,4,c(212,211,184))
for x=9,18,4 do box(im,x,9,2,10,c(192,196,171))end
box(im,3,22,22,6,c(202,205,180));box(im,5,28,3,13,c(139,150,125));box(im,20,28,3,13,c(163,170,146));box(im,5,21,18,2,c(225,223,197));save(im,'CadeiraPlasticaBar')
im=Image(40,56,ColorMode.RGB);box(im,2,1,36,53,c(48,38,28));box(im,4,3,32,48,c(199,190,150));box(im,4,3,32,12,c(106,49,35));
-- Grid of October 2004: starts Friday, thirty-one days. Days are marks; examination transcribes heading.
for y=20,45,5 do for x=7,31,4 do box(im,x,y,2,2,c(102,94,72))end end
box(im,6,18,28,1,c(130,119,87));box(im,7,7,25,2,c(224,204,151));box(im,10,10,19,1,c(181,153,107));box(im,28,35,5,5,c(155,60,42));save(im,'CalendarioOutubro2004')
im=Image(44,56,ColorMode.RGB);box(im,1,1,42,54,c(41,35,28));box(im,3,3,38,50,c(177,161,112));box(im,5,6,34,9,c(71,89,60));
box(im,8,9,28,2,c(212,195,145));ellipse(im,22,31,14,9,c(101,69,40));ellipse(im,22,28,13,7,c(191,143,62));
box(im,10,44,24,2,c(110,75,46));box(im,12,48,20,1,c(110,75,46));box(im,4,4,1,2,c(223,211,168));box(im,39,49,1,2,c(216,207,161));save(im,'CartazGaletoBar')
im=Image(16,24,ColorMode.RGB);ellipse(im,8,21,6,2,c(12,14,12,80));box(im,4,4,8,15,c(133,93,35));box(im,5,3,6,14,c(203,156,64));box(im,4,2,8,3,c(225,211,166));box(im,6,6,1,11,c(239,211,138));save(im,'CopoCervejaBar')
print('Five original bar props exported, with editable sources.')

-- Seated variants retain the original face, torso, palette and pixel density.
for _,name in ipairs({'Npc_Frequentador','Npc_Chico','Npc_Ramiro','Npc_Motorista'})do
 local source=Sprite{fromFile=root..'/Projeto Jogo 2D/Assets/Resources/Art/Characters/'..name..'.png'}
 local original=Image(source.cels[1].image);source:close()
 for frame,suffix in ipairs({'_Sentado','_Bebendo'})do
  local out=Image(original)
  box(out,0,42,out.width,out.height-42,c(0,0,0,0))
  box(out,23,42,18,4,c(55,61,53));box(out,20,45,10,3,c(46,50,44));box(out,35,45,10,3,c(46,50,44))
  box(out,18,48,11,3,c(31,29,24));box(out,36,48,11,3,c(31,29,24));box(out,20,48,7,1,c(113,97,67));box(out,37,48,7,1,c(113,97,67))
  if frame==2 then box(out,36,29,5,7,c(211,177,123));box(out,38,28,4,5,c(186,146,69));box(out,38,27,4,1,c(224,214,179))end
  local doc=Sprite(out.width,out.height,ColorMode.RGB);doc.layers[1].name='Pose sentada / '..name;doc.cels[1].image=out
  doc:saveAs(root..'/Arte/Producao/Bar2004/'..name..suffix..'.aseprite');doc:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Art/Characters/'..name..suffix..'.png');doc:close()
 end
end
