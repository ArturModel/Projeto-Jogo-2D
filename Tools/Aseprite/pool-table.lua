local root=assert(app.params.root);math.randomseed(200431)
local s=Sprite(128,128,ColorMode.RGB);local im=Image(128,128,ColorMode.RGB)
local function color(r,g,b)return app.pixelColor.rgba(r,g,b,255)end
local function rect(x0,y0,x1,y1,r,g,b)
 for y=y0,y1 do for x=x0,x1 do im:drawPixel(x,y,color(r,g,b))end end
end
-- Pernas, saia frontal e tampo em perspectiva discreta. Nada de gradiente borrado.
rect(16,83,28,111,27,23,19);rect(99,83,111,111,27,23,19)
rect(18,84,25,109,81,53,31);rect(101,84,108,109,81,53,31)
rect(8,77,119,92,31,26,21);rect(10,78,117,89,79,49,29)
rect(12,80,115,82,108,73,40);rect(13,87,114,88,44,31,22)
for y=32,79 do
 local spread=math.floor((y-32)/16);local left,right=12-spread,115+spread
 for x=left,right do
  local grain=math.random(-5,5);local bevel=(y<36 or x<left+3) and 16 or 0
  im:drawPixel(x,y,color(78+grain+bevel,51+grain+bevel/2,31+grain))
 end
end
for y=38,73 do
 local spread=math.floor((y-38)/18)
 for x=20-spread,107+spread do
  local n=math.random(-2,2);im:drawPixel(x,y,color(29+n,77+n,52+n))
 end
end
rect(21,37,106,38,19,39,28);rect(18,73,109,75,15,32,23)
local function dot(cx,cy,rx,ry,r,g,b)
 for y=cy-ry,cy+ry do for x=cx-rx,cx+rx do
  if ((x-cx)/rx)^2+((y-cy)/ry)^2<=1 then im:drawPixel(x,y,color(r,g,b))end
 end end
end
for _,p in ipairs({{20,39},{64,38},{107,39},{17,73},{64,74},{110,73}})do
 dot(p[1],p[2],4,3,101,73,40);dot(p[1],p[2]+1,3,2,12,17,14)
end
for _,p in ipairs({{35,54,205,199,174},{88,56,169,111,40},{92,59,116,37,29},{86,62,39,57,73}})do
 dot(p[1],p[2]+1,2,2,16,31,23);dot(p[1],p[2],2,2,p[3],p[4],p[5]);im:drawPixel(p[1]-1,p[2]-1,color(215,209,177))
end
-- Pontos de mira embutidos na madeira, pequenos sinais de uso.
for _,x in ipairs({37,50,78,91})do im:drawPixel(x,34,color(176,160,110));im:drawPixel(x,78,color(151,132,88))end
for i=1,30 do local x,y=math.random(15,111),math.random(83,86);im:drawPixel(x,y,color(92,61,35))end
s.cels[1].image=im;s:saveAs(root..'/MesaSinuca.aseprite');s:saveCopyAs(root..'/MesaSinuca.png');s:close()
print('Mesa de sinuca nativa: pano, moldura, seis caçapas, bolas e pernas.')
