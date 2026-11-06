local root=assert(app.params.root);local pc=app.pixelColor
local function col(r,g,b,a)return pc.rgba(r,g,b,a or 255)end
local function rect(im,x,y,w,h,c)for j=y,y+h-1 do for i=x,x+w-1 do if i>=0 and j>=0 and i<im.width and j<im.height then im:drawPixel(i,j,c)end end end end
local function save(name,kind)
 local s=Sprite(96,80,ColorMode.RGB);local im=Image(96,80,ColorMode.RGB);im:clear(col(0,0,0,0))
 if kind==0 then
 rect(im,13,9,70,59,col(18,22,21));rect(im,16,12,64,52,col(77,83,75));rect(im,20,16,56,12,col(37,42,38))
 for i=0,6 do rect(im,23+i*7,19,3,6,col(17,20,19))end
 rect(im,24,32,47,23,col(30,34,31));rect(im,27,35,41,16,col(111,112,90))
 for _,x in ipairs({36,58})do rect(im,x-6,38,12,10,col(25,30,28));rect(im,x-3,40,6,6,col(159,153,125));rect(im,x-1,41,2,4,col(43,48,44))end
 rect(im,24,59,9,4,col(157,46,35));rect(im,36,59,9,4,col(202,185,135));rect(im,48,59,9,4,col(173,163,126));rect(im,60,59,9,4,col(173,163,126));rect(im,28,6,40,3,col(24,28,26))
 elseif kind==1 then
 rect(im,14,20,68,40,col(29,31,27));rect(im,17,23,62,33,col(91,84,65));rect(im,19,25,58,12,col(201,188,145));rect(im,24,28,24,2,col(76,79,67));rect(im,24,32,35,1,col(115,106,80));
 for _,x in ipairs({33,63})do rect(im,x-7,39,14,12,col(24,28,25));rect(im,x-3,42,6,6,col(184,178,140))end
 rect(im,39,47,18,4,col(35,36,30));rect(im,26,55,44,3,col(40,42,34))
 else rect(im,25,8,48,65,col(187,174,135));rect(im,28,11,42,57,col(222,209,167));for y=22,55,6 do rect(im,34,y,29,1,col(105,100,80))end end
 s:newCel(s.layers[1],s.frames[1],im,Point(0,0));s:saveAs(root..'/Arte/Producao/Caderno/'..name..'.aseprite');s:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Tapes/'..name..'.png');s:close()
end
save('Recorder',0);save('Cassette',1);save('Letter',2)
