void raymarch_float(float3 ray_origin, float3 ray_direction, float num_steps, float step_size,
                    float density_scale, UnityTexture3D volume_tex, UnitySamplerState volume_sampler,
                    float3 offset, float num_light_steps, float light_step_size, float3 light_dir,
                    float light_absorb, float darkness_threshold, float transmittance, out float3 result)
{
    float density = 0;
    float light_accumulation = 0;
    float final_light = 0;

    for (int i = 0; i < num_steps; i++)
    {
        ray_origin += (ray_direction * step_size);

        float3 sample_pos = ray_origin + offset;
        float sampled_density = SAMPLE_TEXTURE3D(volume_tex, volume_sampler, sample_pos).r;
        density += sampled_density * density_scale;

        float3 light_ray_origin = sample_pos;

        for (int j = 0; j < num_light_steps; j++)
        {
            light_ray_origin += -light_dir * light_step_size;
            float light_density = SAMPLE_TEXTURE3D(volume_tex, volume_sampler, light_ray_origin).r;
            light_accumulation += light_density;
        }

        float light_transmission = exp(-light_accumulation);
        float shadow = darkness_threshold + light_transmission * (1.0 - darkness_threshold);
        final_light += density * transmittance * shadow;
        transmittance *= exp(-density * light_absorb);
    }

    float transmission = exp(-density);
    result = float3(final_light, transmission, transmittance);
}
