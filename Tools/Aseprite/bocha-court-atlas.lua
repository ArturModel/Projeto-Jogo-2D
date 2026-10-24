-- Materiais nativos de Santa Rita para a projeção da cancha. Fonte editável;
-- síntese de granulado e veios, sem assets/código do jogo de referência.
local root=assert(app.params.root);local pc=app.pixelColor
local im=Image(256,128,ColorMode.RGB);math.randomseed(2004)
local function rgb(r,g,b)return pc.rgba(math.floor(r),math.floor(g),math.floor(b),255)end
for y=0,127 do for x=0,255 do im:drawPixel(x,y,rgb(255,255,255))end end
for tile=0,3 do for y=0,63 do for x=0,63 do
 local r,g,b
 if tile==0 then
  local grain=math.random(-4,4);local rake=math.sin((y+x*.06)*.55)*1.5
  local pebble=math.random(1,80)==1 and math.random(8,14) or 0
  r=137+grain+rake+pebble;g=113+grain+rake+pebble*.8;b=76+grain+rake+pebble*.6
 elseif tile==1 then
  local seam=(x%16==0 or x%16==15) and -19 or 0
  local knot=math.sin(y*.1+x*.9)*2
  local grain=math.sin(y*.33+x*.61)*4+knot+math.random(-2,2)+(math.floor(x/16)%3-1)*5
  local edge=x%16==1 and 7 or 0
  r=98+seam+grain+edge;g=66+seam*.7+grain+edge*.7;b=42+seam*.4+grain+edge*.4
 elseif tile==2 then
  local grain=math.random(-3,3);local stain=math.sin(x*.09)*math.sin(y*.08)*3
  r=125+grain+stain;g=122+grain+stain;b=99+grain+stain
 else r=255;g=255;b=255 end
 im:drawPixel(tile*64+x,y,rgb(r,g,b))
end end end
local sprite=Sprite(256,128,ColorMode.RGB);sprite.layers[1].name='Saibro · Madeira · Reboco · Cor sólida'
sprite:newCel(sprite.layers[1],1,im,Point(0,0))
sprite:saveAs(root..'/Arte/Producao/Bocha/CanchaAtlas.aseprite')
sprite:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Bocha/CanchaAtlas.png')
sprite:close();print('CanchaAtlas: materiais originais exportados')
-- Mesma matéria na cena explorável e na vista da partida, em células de 32 px.
for tile,name in pairs({[0]='SaibroCancha',[1]='MadeiraCancha'})do
 local cut=Image(32,32,ColorMode.RGB)
 for y=0,31 do for x=0,31 do cut:drawPixel(x,y,im:getPixel(tile*64+x,y))end end
 local s=Sprite(32,32,ColorMode.RGB);s.cels[1].image=cut
 s:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Bocha/'..name..'.png');s:close()
end
local lamp=Image(32,48,ColorMode.RGB)
local function box(x,y,w,h,r,g,b)for yy=y,y+h-1 do for xx=x,x+w-1 do lamp:drawPixel(xx,yy,rgb(r,g,b))end end end
box(13,7,7,22,34,31,27);box(15,8,3,18,92,80,59);box(15,16,13,4,39,36,29)
box(25,16,3,9,92,82,61);box(8,26,22,3,27,27,23);box(11,23,16,3,63,65,55)
box(10,29,18,8,33,34,28);box(12,30,14,5,229,191,105);box(13,30,12,2,250,222,147)
box(11,36,17,2,89,78,48);box(14,38,11,1,48,42,30)
local l=Sprite(32,48,ColorMode.RGB);l.layers[1].name='Arandela de chapa · difusor quente';l.cels[1].image=lamp
l:saveAs(root..'/Arte/Producao/Bocha/LuminariaCancha.aseprite');l:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Bocha/LuminariaCancha.png');l:close()
