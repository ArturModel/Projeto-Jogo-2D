-- Montagem de capturas reais; redução inteira mantém a leitura original dos pixels.
local folder=assert(app.params.folder);local prefix=assert(app.params.prefix);local count=tonumber(assert(app.params.count));local output=assert(app.params.output)
local step=tonumber(app.params.step or '1');local rate=tonumber(app.params.rate or '15')
local first=Image{fromFile=folder..'/'..prefix..'-000.png'};local w,h=first.width,first.height
local scale=math.max(1,math.ceil(w/640));local close=app.params.closeup=='true'
local result=Sprite(close and 384 or math.floor(w/scale),close and 320 or math.floor(h/scale),ColorMode.RGB)
local times={};local log=io.open(folder..'/captura.csv','r')
if log then for line in log:lines() do local name,index,time=line:match('^([^,]+),(%d+),([%d%.]+),');if name==prefix then times[tonumber(index)]=tonumber(time) end end;log:close() end
local frame=0
for i=0,count-1,step do
 local im=Image{fromFile=folder..'/'..prefix..'-'..string.format('%03d',i)..'.png'};local reduced=Image(result.width,result.height,ColorMode.RGB)
 for y=0,result.height-1 do for x=0,result.width-1 do
  local sx=close and math.floor(w/2)-96+math.floor(x/2) or x*scale
  local sy=close and math.floor(h/2)-60+math.floor(y/2) or y*scale
  reduced:drawPixel(x,y,im:getPixel(sx,sy))
 end end
 frame=frame+1;if frame>1 then result:newEmptyFrame() end
 result:newCel(result.layers[1],frame,reduced,Point(0,0));result.frames[frame].duration=times[i] and times[i+step] and math.max(.01,times[i+step]-times[i]) or step/rate
end
result:saveCopyAs(output);result:close();print('GIF de câmera real exportado: '..prefix)
