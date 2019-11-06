#pragma once
#include <memory>
#include "Image.h"
#include <gli/gli.hpp>

class GliImageBase : public image::IImage
{
protected:
	GliImageBase(gli::texture& base, gli::format original) : m_base(base), m_original(original) {}
public:
	virtual ~GliImageBase() override = default;
	uint32_t getNumLayers() const override final { return uint32_t(m_base.layers() * m_base.faces()); }
	uint32_t getNumMipmaps() const override final { return uint32_t(m_base.levels()); }
	uint32_t getWidth(uint32_t mipmap) const override final { return m_base.extent(mipmap).x; }
	uint32_t getHeight(uint32_t mipmap) const override final { return m_base.extent(mipmap).y; }
	gli::format getFormat() const override final { return m_base.format(); }
	gli::format getOriginalFormat() const override final { return m_original; }
	uint8_t* getData(uint32_t layer, uint32_t mipmap, uint32_t& size) override final
	{
		size = uint32_t(m_base.size(mipmap));
		auto gliface = layer % m_base.faces();
		auto glilayer = layer / m_base.faces();

		return reinterpret_cast<uint8_t*>(m_base.data(glilayer, gliface, mipmap));
	}
	const uint8_t* getData(uint32_t layer, uint32_t mipmap, uint32_t& size) const override final
	{
		return const_cast<GliImageBase*>(this)->getData(layer, mipmap, size);
	}

	uint8_t* getData() { return reinterpret_cast<uint8_t*>(m_base.data()); }
	uint32_t getSize() { return uint32_t(m_base.size()); }
protected:
	gli::texture& m_base;
	gli::format m_original;
};

class GliImage final : public GliImageBase
{
public:
	GliImage(const gli::texture& tex);
	GliImage(gli::format format, gli::format original, size_t nLayer, size_t nFaces, size_t nLevel, size_t width, size_t height);
	GliImage(gli::format format, size_t nLayer, size_t nLevel, size_t width, size_t height);
	GliImage(const gli::texture& tex, gli::format original);

	std::unique_ptr<GliImage> convert(gli::format format, int quality);
	void saveKtx(const char* filename) const;
	void saveDds(const char* filename) const;
	void flip();
private:
	// helper to choose the correct internal format. This is later required for format conversions
	gli::texture& initTex(size_t nFaces);

	gli::texture_cube_array m_cube;
	gli::texture2d_array m_array;
	bool m_isCube;
};