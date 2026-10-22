-- Asfalto em escala de pixel: agregado fino, remendos e fissuras; sem ladrilhos/pedras grandes.
local root=assert(app.params.root);local tiles=assert(app.params.tiles);math.randomseed(200403)
local pc=app.pixelColor;local function rgb(r,g,b)return pc.rgba(r,g,b,255)end
local im=Image(128,128,ColorMode.RGB)
for y=0,127 do for x=0,127 do
  local patch=math.sin(x*math.pi/32)*math.cos(y*math.pi/64)*1.5
  local grain=math.random(-3,3);local v=math.floor(43+patch+grain)
  im:drawPixel(x,y,rgb(v,v+1,v+2))
end end
for i=1,900 do
  local x,y=math.random(0,127),math.random(0,127);local v=math.random(47,55)
  im:drawPixel(x,y,rgb(v,v+1,v+1))
  if i%5==0 then im:drawPixel((x+1)%128,y,rgb(v-4,v-3,v-3)) end
end
for n=1,3 do
  local x,y=math.random(0,127),math.random(0,127)
  for j=1,math.random(13,25) do
    im:drawPixel(x%128,y%128,rgb(29,30,31))
    x=x+math.random(-1,1);y=y+1
  end
end
local spr=Sprite(128,128,ColorMode.RGB);spr.cels[1].image=im
spr:saveAs(root..'/Asfalto_Macro.aseprite');spr:saveCopyAs(root..'/Asfalto_Macro.png');spr:close()
for y=0,3 do for x=0,3 do
  local tile=Sprite(32,32,ColorMode.RGB);local pixels=Image(32,32,ColorMode.RGB)
  for py=0,31 do for px=0,31 do pixels:drawPixel(px,py,im:getPixel(x*32+px,y*32+py)) end end
  tile.cels[1].image=pixels;tile:saveCopyAs(root..'/Asfalto_'..x..'_'..(3-y)..'.png');tile:close()
end end
for _,name in ipairs({'Asfalto','AsfaltoH','AsfaltoV'}) do
  local tile=Sprite(32,32,ColorMode.RGB);local pixels=Image(32,32,ColorMode.RGB)
  for y=0,31 do for x=0,31 do
    local p=im:getPixel(32+x,32+y)
    if (name=='AsfaltoH' and (y==14 or y==17)) or (name=='AsfaltoV' and (x==14 or x==17)) then
      p=rgb(177+math.random(-8,5),143+math.random(-6,6),57)
    end
    pixels:drawPixel(x,y,p)
  end end
  tile.cels[1].image=pixels;tile:saveAs(tiles..'/'..name..'.aseprite');tile:saveCopyAs(tiles..'/'..name..'.png');tile:close()
end
print('Asfalto nativo: agregado fino, 16 células contínuas e marcação dupla.')
