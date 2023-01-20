install.packages("magrittr") # package installations are only needed the first time you use it
install.packages("dplyr")    # alternative installation of the %>%
install.packages("kableExtra") 
install.packages("tidyverse")
install.packages("ggplot2")
library(magrittr) # needs to be run every time you start R and want to use %>%
library(dplyr)    # alternatively, this also loads %>%
library(tibble)
library(knitr)
library(kableExtra)
library(tidyverse)
library(ggplot2)



#file <- "icons.txt"
file <- "textures.txt"
#file <- "nat173.txt"
#file <- "arti.txt"

d = read.table(file, header = TRUE, sep = "\t", stringsAsFactors=TRUE,dec = ".")
d %>%
  mutate_if(is.character, trimws)-> d


#compression time
compTime<- d %>% group_by(Type) %>% summarise(Best=min(CompressionTime.ms.),Worst=max(CompressionTime.ms.),
                                              Average=round(mean(CompressionTime.ms.),digits=3))
compTime <- data.frame(compTime)
rownames(compTime) <- unique(compTime$Type)
compTime <- t(compTime)
compTime <- compTime[-1,]

#tables
kable(head(compTime), format = "html", digits = 5) %>% kable_styling(bootstrap_options="striped")

#decompTime
decompTime<- d %>% group_by(Type) %>% summarise(Best=min(DecompressionTime.ms.),Worst=max(DecompressionTime.ms.),Average=round(mean(DecompressionTime.ms.),digits=3))
decompTime <- data.frame(decompTime)
rownames(decompTime) <- unique(decompTime$Type)
decompTime <- t(decompTime)
decompTime <- decompTime[-1,]
kable(head(decompTime), format = "html", digits = 5) %>% kable_styling(bootstrap_options="striped")

#compression rate
decompTime<- d %>% group_by(Type) %>% summarise(Best=max(CompressionRate),Worst=min(CompressionRate),Average=round(mean(CompressionRate),digits=3))
decompTime <- data.frame(decompTime)
rownames(decompTime) <- unique(decompTime$Type)
decompTime <- t(decompTime)
decompTime <- decompTime[-1,]
kable(head(decompTime), format = "html", digits = 5) %>% kable_styling(bootstrap_options="striped")


#plot
plot(d$Size.bytes.,d$CompressionRate,col=d$Type, type="b")

d$quartile <- ntile(d$Size.bytes.,4)
d %>% group_by(Size.bytes.) -> grp
d %>% summarise(Size.bytes.) -> sum

ggplot(d,aes(CompressionRate,quartile,colour=Type))+geom_point()+ylab("Size (bytes) grouped by quartile")+xlab("Compression Ratio")
#+geom_line()


d$quartile <- ntile(d$CompressionRate.,4)
d %>% group_by(CompressionRate) -> grp
d %>% summarise(CompressionRate) -> sum

ggplot(d,aes(quartile,Size.bytes.,colour=Type))+geom_point()+xlab("Compression Rate grouped by quartiles")+ylab("Size (bytes)")




ggplot(d,aes(Size.bytes.,CompressionRate,colour=Type))+geom_point()+xlab("Size (bytes)")+ylab("Compression Ratio")
p <- ggplot(d,aes(Type,CompressionRate,colour=Size.bytes.))+geom_point()+xlab("Size (bytes)")+ylab("Compression Ratio")
p

p<- ggplot(d,aes(Size.bytes.,CompressionRate,colour=Type))+geom_boxplot()
p

p<- ggplot(d,aes(Type,CompressionRate,colour=Size.bytes.))+geom_boxplot()
p

p<- ggplot(d,aes(CompressionRate,Size.bytes.,colour=Type))+geom_boxplot()
p
