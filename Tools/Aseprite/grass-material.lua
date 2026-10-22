-- Material nativo em pixels: pequenos tufos agrupados e solo, sem interpolação.
local root=assert(app.params.root);local size=128
math.randomseed(20041019)
local sprite=Sprite(size,size,ColorMode.RGB);local image=Image(size,size,ColorMode.RGB)
local function rgb(r,g,b) return app.pixelColor.rgba(r,g,b,255) end
local soil={rgb(43,40,29),rgb(47,44,31),rgb(50,47,33),rgb(53,49,34)}
local grass={rgb(31,42,30),rgb(39,51,32),rgb(51,62,36),rgb(66,73,41),rgb(78,82,46)}
local function patch(x,y) return math.sin(x*.075)+math.cos(y*.12)+math.sin((x+y)*.06) end
for y=0,127 do for x=0,127 do
  local n=patch(x,y);image:drawPixel(x,y,soil[math.max(1,math.min(4,math.floor(n+2.5)))])
end end
local function put(x,y,color) image:drawPixel(x%size,y%size,color) end
for i=1,2100 do
  local x,y=math.random(0,127),math.random(0,127)
  if patch(x,y)>-.9 or math.random()<.23 then
    local shade=math.random(1,3);local height=math.random(1,3)
    put(x,y,grass[1]);put(x+1,y,grass[2]);put(x,y-height,grass[shade+1])
    if height>1 then put(x-1,y-1,grass[shade]);put(x+1,y-1,grass[shade+1]) end
    if i%5==0 then put(x+1,y-height,grass[shade+2]) end
  end
end
for i=1,32 do local x,y=math.random(0,127),math.random(0,127);put(x,y,rgb(76,71,53));put(x+1,y,rgb(55,52,40)) end
sprite.cels[1].image=image;sprite:saveAs(root.."/Grama_Macro.aseprite");sprite:saveCopyAs(root.."/Grama_Macro.png")
for y=0,3 do for x=0,3 do
  local tile=Sprite(32,32,ColorMode.RGB);local pixels=Image(32,32,ColorMode.RGB)
  for py=0,31 do for px=0,31 do pixels:drawPixel(px,py,image:getPixel(x*32+px,y*32+py)) end end
  tile.cels[1].image=pixels;tile:saveCopyAs(root.."/Grama_"..x.."_"..(3-y)..".png");tile:close()
end end
sprite:close();print("Grama nativa em grupos de pixels exportada")
